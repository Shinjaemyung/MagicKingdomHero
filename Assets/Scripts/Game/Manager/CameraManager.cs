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

    [SerializeField, Tooltip("Hero 모드 가상 카메라")]
    CinemachineVirtualCamera heroFollowCamera;

    [SerializeField, Tooltip("타워 배치 모드 가상 카메라")]
    CinemachineVirtualCamera towerPlacementCamera;


    /// <summary>카메라 블렌드 중인지 여부 반환</summary>
    public bool IsBlending => cinemachineBrain.IsBlending;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        cinemachineBrain = FindAnyObjectByType<CinemachineBrain>();

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

    public void SetTowerPlacementModeView()
    {
        // 카메라가 Hero를 바라보도록 위치 변경
        Transform heroTransform = heroFollowCamera.Follow;
        if (heroTransform != null)
        {
            Vector3 heroPos = heroTransform.position;
            float camY = towerPlacementCamera.transform.position.y;
            Vector3 dir = -towerPlacementCamera.transform.forward;
            float t = (camY - heroPos.y) / dir.y;
            towerPlacementCamera.transform.position = heroPos + dir * t;
        }

        towerPlacementCamera.Priority = 20;
        heroFollowCamera.Priority = 10;
    }

    public void SetHeroControlModeView()
    {
        heroFollowCamera.Priority = 20;
        towerPlacementCamera.Priority = 10;
    }
}
