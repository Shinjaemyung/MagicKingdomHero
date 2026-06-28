using System.Collections;
using Cinemachine;
using UnityEngine;
using static PlayerModeManager;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 startRotation;

    float mousePosX;
    float mousePosY;

    float moveScreenEdgeThickness = 10f;

    [SerializeField] float moveSpeed = 20f;
    [SerializeField] Vector2 cameraRangeX = new Vector2(-50f, 50f);
    [SerializeField] Vector2 cameraRangeZ = new Vector2(-50f, 50f);

    float currentZoom;

    [SerializeField] float minZoom = 20f;
    [SerializeField] float maxZoom = 80f;
    [SerializeField] float zoomSpeed = 4000f;

    CinemachineBrain cinemachineBrain;
    CinemachineImpulseSource impulseSource;

    [SerializeField, Tooltip("Hero 모드 가상 카메라")]
    CinemachineVirtualCamera heroFollowCamera;

    [SerializeField, Tooltip("타워 배치 모드 가상 카메라")]
    CinemachineVirtualCamera towerPlacementCamera;

    /// <summary>true면 TowerPlacementCamera가 특정 위치에 고정되어 마우스 이동 입력을 무시한다.</summary>
    bool isTowerPlacementCameraLocked;

    Coroutine _moveCameraCoroutine;

    /// <summary>카메라 블렌드 중인지 여부 반환</summary>
    public bool IsBlending => cinemachineBrain.IsBlending;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        cinemachineBrain = FindAnyObjectByType<CinemachineBrain>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        currentZoom = towerPlacementCamera.m_Lens.FieldOfView;
    }

    private void Start()
    {
        towerPlacementCamera.transform.position = startPosition;
        towerPlacementCamera.transform.eulerAngles = startRotation;
    }

    private void Update()
    {
        mousePosX = Input.mousePosition.x;
        mousePosY = Input.mousePosition.y;

        PlayerMode currentPlayerMode = PlayerModeManager.Instance.playerMode;
        if (currentPlayerMode == PlayerMode.TowerPlacementMode)
        {
            if (!isTowerPlacementCameraLocked)
                MoveCamera();

            ZoomCamera();
        }
    }

    void MoveCamera()
    {
        Vector3 direction = Vector3.zero;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (mousePosX <= moveScreenEdgeThickness)
        {
            direction += Vector3.left;
        }

        if (mousePosX >= screenWidth - moveScreenEdgeThickness)
        {
            direction += Vector3.right;
        }

        if (mousePosY <= moveScreenEdgeThickness)
        {
            direction += Vector3.back;
        }

        if (mousePosY >= screenHeight - moveScreenEdgeThickness)
        {
            direction += Vector3.forward;
        }

        if (direction != Vector3.zero)
        {
            direction = direction.normalized;

            Vector3 newPosition = towerPlacementCamera.transform.position + direction * moveSpeed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, cameraRangeX.x, cameraRangeX.y);
            newPosition.z = Mathf.Clamp(newPosition.z, cameraRangeZ.x, cameraRangeZ.y);

            towerPlacementCamera.transform.position = newPosition;
        }
    }

    void ZoomCamera()
    {
        float scroollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (scroollWheel == 0)
            return;

        float zoomAmount = scroollWheel * zoomSpeed * Time.deltaTime;
        currentZoom -= zoomAmount;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        towerPlacementCamera.m_Lens.FieldOfView = currentZoom;
    }

    /// <summary>
    /// TowerPlacementCamera의 현재 각도를 유지한 채, targetPosition을 바라보게 되는 위치를 계산한다.
    /// (카메라 높이는 그대로 유지하고, 바라보는 방향의 직선이 targetPosition을 지나도록 위치를 옮긴다)
    /// </summary>
    Vector3 CalculatePositionLookingAt(Vector3 targetPosition)
    {
        float camY = towerPlacementCamera.transform.position.y;
        Vector3 dir = -towerPlacementCamera.transform.forward;
        float t = (camY - targetPosition.y) / dir.y;
        return targetPosition + dir * t;
    }

    public void SetTowerPlacementModeView()
    {
        // 카메라가 Hero를 바라보도록 위치 변경
        Transform heroTransform = heroFollowCamera.Follow;
        if (heroTransform != null)
        {
            towerPlacementCamera.transform.position = CalculatePositionLookingAt(heroTransform.position);
        }

        towerPlacementCamera.Priority = 20;
        heroFollowCamera.Priority = 10;
    }

    public void SetHeroControlModeView()
    {
        heroFollowCamera.Priority = 20;
        towerPlacementCamera.Priority = 10;
    }

    /// <summary>
    /// TowerPlacementCamera를 targetPosition을 바라보는 위치로 duration초에 걸쳐 부드럽게 이동시키고,
    /// 도착 후 고정한다. (바라보는 방식은 SetTowerPlacementModeView와 동일)
    /// </summary>
    /// <param name="targetPosition">카메라가 바라봐야 할 지점</param>
    /// <param name="duration">이동에 걸리는 시간(초)</param>
    public void MoveTowerPlacementCameraTo(Vector3 targetPosition, float duration = 1.5f)
    {
        if (_moveCameraCoroutine != null)
            StopCoroutine(_moveCameraCoroutine);

        _moveCameraCoroutine = StartCoroutine(MoveTowerPlacementCameraRoutine(targetPosition, duration));
    }

    IEnumerator MoveTowerPlacementCameraRoutine(Vector3 targetPosition, float duration)
    {
        LockTowerPlacementCamera();

        Vector3 startPos = towerPlacementCamera.transform.position;
        Vector3 endPos = CalculatePositionLookingAt(targetPosition);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            towerPlacementCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        towerPlacementCamera.transform.position = endPos;
        UnlockTowerPlacementCamera();
        _moveCameraCoroutine = null;
    }

    public void LockTowerPlacementCamera()
    {
        isTowerPlacementCameraLocked = true;
    }

    public void UnlockTowerPlacementCamera()
    {
        isTowerPlacementCameraLocked = false;
    }

    /// <summary>카메라 흔들림</summary>
    /// <param name="force">흔들림 강도</param>
    public void ShakeCamera(float force = 0.5f)
    {
        if (impulseSource == null) return;
        impulseSource.GenerateImpulseWithForce(force);
    }
}
