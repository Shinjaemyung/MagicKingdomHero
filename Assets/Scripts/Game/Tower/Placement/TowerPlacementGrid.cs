using System;
using Core.Utilities;
using TowerDefense.UI.HUD;
using UnityEngine;

namespace TowerDefense.Towers.Placement
{
    /// <summary>
    /// 그리드로 구성된 타워 배치 영역
    /// 원점은 오른쪽 아래 셀의 중앙에 위치하며, 어떤 방향으로도 배치 가능
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class TowerPlacementGrid : MonoBehaviour, IPlacementArea
    {
        /// <summary>
        /// 그리드를 시각적으로 표시하는 데 사용되는 프리팹
        /// </summary>
        public PlacementTile placementTilePrefab;

        /// <summary>
        /// 그리드의 크기(가로, 세로 셀 수)
        /// </summary>
        public IntVector2 dimensions;

        /// <summary>
        /// 하나의 셀 변의 길이
        /// </summary>
        [Tooltip("이 영역에서 사용되는 하나의 그리드 셀 크기. 타워의 실제 그리드 크기와 일치해야 함")]
        public float gridSize = 1;

        /// <summary>
        /// gridSize의 역수(곱셈용)
        /// </summary>
        float _invGridSize;

        /// <summary>
        /// 사용 가능한 셀 배열
        /// </summary>
        bool[,] _availableCells;

        /// <summary>
        /// PlacementTile 배열
        /// </summary>
        PlacementTile[,] _tiles;

        /// <summary>
        /// 월드 좌표를 그리드 좌표로 변환
        /// </summary>
        /// <param name="worldLocation"><see cref="Vector3"/> 변환할 월드 좌표</param>
        /// <param name="sizeOffset"><see cref="IntVector2"/> 배치할 오브젝트 크기 보정값</param>
        /// <returns><see cref="IntVector2"/> 해당 위치에 대응되는 그리드 좌표</returns>
        public IntVector2 WorldToGrid(Vector3 worldLocation, IntVector2 sizeOffset)
        {
            Vector3 localLocation = transform.InverseTransformPoint(worldLocation);

            // 그리드 크기의 역수를 곱해서 좌표를 변환
            localLocation *= _invGridSize;

            // 오브젝트 크기의 절반만큼 위치 보정
            var offset = new Vector3(sizeOffset.x * 0.5f, 0.0f, sizeOffset.y * 0.5f);
            localLocation -= offset;

            int xPos = Mathf.RoundToInt(localLocation.x);
            int yPos = Mathf.RoundToInt(localLocation.z);

            return new IntVector2(xPos, yPos);
        }

        /// <summary>
        /// 그리드 좌표에 대응되는 월드 좌표를 반환
        /// </summary>
        /// <param name="gridPosition">그리드 공간상의 좌표</param>
        /// <param name="sizeOffset"><see cref="IntVector2"/>배치할 오브젝트 크기 보정값</param>
        /// <returns>지정된 셀의 월드 좌표</returns>
        public Vector3 GridToWorld(IntVector2 gridPosition, IntVector2 sizeOffset)
        {
            // 그리드 크기를 적용한 로컬 좌표 계산
            Vector3 localPos = new Vector3(gridPosition.x + (sizeOffset.x * 0.5f), 0, gridPosition.y + (sizeOffset.y * 0.5f)) *
                               gridSize;

            return transform.TransformPoint(localPos);
        }

        /// <summary>
        /// 지정한 셀 범위가 타워 배치 가능한 위치인지 검사
        /// </summary>
        /// <param name="gridPos">그리드 위치</param>
        /// <param name="size">오브젝트 크기</param>
        /// <returns>배치 가능 여부</returns>
        public TowerFitStatus GetFits(IntVector2 gridPos, IntVector2 size)
        {
            // 타워의 타일 크기가 배치 영역보다 크면 배치 불가
            if ((size.x > dimensions.x) || (size.y > dimensions.y))
            {
                return TowerFitStatus.OutOfBounds;
            }

            IntVector2 extents = gridPos + size;

            // 배치 영역 범위를 벗어나는 경우 배치 불가
            if ((gridPos.x < 0) || (gridPos.y < 0) ||
                (extents.x > dimensions.x) || (extents.y > dimensions.y))
            {
                return TowerFitStatus.OutOfBounds;
            }

            // 해당 영역 안에 이미 배치된 타워가 있다면 배치 불가
            for (int y = gridPos.y; y < extents.y; y++)
            {
                for (int x = gridPos.x; x < extents.x; x++)
                {
                    if (_availableCells[x, y])
                    {
                        return TowerFitStatus.Overlaps;
                    }
                }
            }

            // 문제가 없다면 배치 가능한 위치
            return TowerFitStatus.Fits;
        }

        /// <summary>
        /// 셀 범위를 타워가 점유 중인 상태로 설정
        /// </summary>
        /// <param name="gridPos">그리드 위치</param>
        /// <param name="size">오브젝트 크기</param>
        public void Occupy(IntVector2 gridPos, IntVector2 size)
        {
            IntVector2 extents = gridPos + size;

            // 크기 유효성 검사
            if ((size.x > dimensions.x) || (size.y > dimensions.y))
            {
                throw new ArgumentOutOfRangeException("size", "Given dimensions do not fit in our grid");
            }

            // 그리드 범위를 벗어남
            if ((gridPos.x < 0) || (gridPos.y < 0) ||
                (extents.x > dimensions.x) || (extents.y > dimensions.y))
            {
                throw new ArgumentOutOfRangeException("gridPos", "Given footprint is out of range of our grid");
            }

            // 해당 영역을 점유 상태로 표시
            for (int y = gridPos.y; y < extents.y; y++)
            {
                for (int x = gridPos.x; x < extents.x; x++)
                {
                    _availableCells[x, y] = true;

                    if (_tiles != null && _tiles[x, y] != null)
                    {
                        _tiles[x, y].SetState(PlacementTileState.Filled);
                    }
                }
            }
        }

        /// <summary>
        /// 타워를 제거하고 해당 셀들을 비어 있는 상태로 되돌림
        /// </summary>
        /// <param name="gridPos">그리드 위치</param>
        /// <param name="size">오브젝트 크기</param>
        public void Clear(IntVector2 gridPos, IntVector2 size)
        {
            IntVector2 extents = gridPos + size;

            // 크기 유효성 검사
            if ((size.x > dimensions.x) || (size.y > dimensions.y))
            {
                throw new ArgumentOutOfRangeException("size", "Given dimensions do not fit in our grid");
            }

            // 그리드 범위를 벗어남
            if ((gridPos.x < 0) || (gridPos.y < 0) ||
                (extents.x > dimensions.x) || (extents.y > dimensions.y))
            {
                throw new ArgumentOutOfRangeException("gridPos", "Given footprint is out of range of our grid");
            }

            // 해당 영역을 비어 있는 상태로 표시
            for (int y = gridPos.y; y < extents.y; y++)
            {
                for (int x = gridPos.x; x < extents.x; x++)
                {
                    _availableCells[x, y] = false;

                    if (_tiles != null && _tiles[x, y] != null)
                    {
                        _tiles[x, y].SetState(PlacementTileState.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        protected virtual void Awake()
        {
            ResizeCollider();

            // 빈 bool 배열 생성 (기본값은 false)
            _availableCells = new bool[dimensions.x, dimensions.y];

            // 좌표 변환 시마다 나눗셈을 하지 않도록
            // 미리 gridSize의 역수를 계산해 둠
            _invGridSize = 1 / gridSize;

            SetUpGrid();
        }

        /// <summary>
        /// Collider의 크기와 중심점 설정
        /// </summary>
        void ResizeCollider()
        {
            var myCollider = GetComponent<BoxCollider>();
            Vector3 size = new Vector3(dimensions.x, 0, dimensions.y) * gridSize;
            myCollider.size = size;

            // Collider의 원점은 좌하단 모서리
            myCollider.center = size * 0.5f;
        }

        /// <summary>
        /// 그리드를 시각화하기 위한 Tile 오브젝트를 생성하고 _availableCells 초기화
        /// </summary>
        protected void SetUpGrid()
        {
            PlacementTile tileToUse;
#if UNITY_STANDALONE
            tileToUse = placementTilePrefab;
#endif

            if (tileToUse != null)
            {
                // 셀들을 담을 Container 부모 오브젝트 생성
                var tilesParent = new GameObject("Container");
                tilesParent.transform.parent = transform;
                tilesParent.transform.localPosition = Vector3.zero;
                tilesParent.transform.localRotation = Quaternion.identity;
                _tiles = new PlacementTile[dimensions.x, dimensions.y];

                for (int y = 0; y < dimensions.y; y++)
                {
                    for (int x = 0; x < dimensions.x; x++)
                    {
                        Vector3 targetPos = GridToWorld(new IntVector2(x, y), new IntVector2(1, 1));
                        targetPos.y += 0.01f;
                        PlacementTile newTile = Instantiate(tileToUse);
                        newTile.transform.parent = tilesParent.transform;
                        newTile.transform.position = targetPos;
                        newTile.transform.localRotation = Quaternion.identity;

                        _tiles[x, y] = newTile;
                        newTile.SetState(PlacementTileState.Empty);
                    }
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 에디터/인스펙터에서 값이 변경될 때 Collider 크기를 올바르게 유지
        /// Collider 컴포넌트는 숨겨서 사용자가 실수로 수정하지 못하게 함
        /// 사용자가 직접 수정할 필요가 없는 값들
        /// </summary>
        void OnValidate()
        {
            // 그리드 크기 검증
            if (gridSize <= 0)
            {
                Debug.LogError("그리드 크기가 음수이거나 0일 수 없음");
                gridSize = 1;
            }

            // 그리드 크기(가로/세로 셀 수) 검증
            if (dimensions.x <= 0 ||
                dimensions.y <= 0)
            {
                Debug.LogError("그리드 크기가 음수이거나 0일 수 없음");
                dimensions = new IntVector2(Mathf.Max(dimensions.x, 1), Mathf.Max(dimensions.y, 1));
            }

            // Collider 크기를 올바르게 갱신
            ResizeCollider();

            // Collider를 인스펙터에서 숨김
            GetComponent<BoxCollider>().hideFlags = HideFlags.HideInInspector;
        }

        /// <summary>
        /// 씬 뷰에서 그리드를 그려줌
        /// </summary>
        void OnDrawGizmos()
        {
            Color prevCol = Gizmos.color;
            Gizmos.color = Color.cyan;

            Matrix4x4 originalMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            // 로컬 공간 기준의 납작한 격자 큐브 그리기
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    var position = new Vector3((x + 0.5f) * gridSize, 0, (y + 0.5f) * gridSize);
                    Gizmos.DrawWireCube(position, new Vector3(gridSize, 0, gridSize));
                }
            }

            Gizmos.matrix = originalMatrix;
            Gizmos.color = prevCol;

            // 배치 영역 중앙에 아이콘도 함께 표시
            Vector3 center = transform.TransformPoint(new Vector3(gridSize * dimensions.x * 0.5f,
                                                                  1,
                                                                  gridSize * dimensions.y * 0.5f));
            Gizmos.DrawIcon(center, "build_zone.png", true);
        }
#endif
    }
}