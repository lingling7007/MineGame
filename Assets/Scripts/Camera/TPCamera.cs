using System;
using System.Collections;
using UnityEngine;

public class TPCamera : MonoBehaviour
{
    public enum ECameraView
    {
        FreeView = 1,
        LockView = 2,
        Lock2P5View = 3,
    }

    private static TPCamera _instance;
    public static TPCamera instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TPCamera>();
                if (_instance == null)
                    return null;
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    #region inspector properties
    public Transform lockTarget;
    public float mouseX = 0f;
    public float mouseY = 0f;
    public float distance = 5f;
    [Range(0f, 10f)]
    public float X_MouseSensitivity = 3f;
    [Range(0f, 10f)]
    public float Y_MouseSensitivity = 3f;
    [Range(0f, 1f)]
    public float S_MouseSensitivity = 0.05f;
    [Range(0f, 1f)]
    public float SmoothBetweenState = 0.05f;

    private float currentSmoothBetweenStateTime = 0.0f;
    private bool isLerpState = false;

    [Range(0f, 20f)]
    public float SmoothCameraRotation = 12f;
    public float SmoothFollow = 10f;
    public float Y_MinLimit = -40f;
    public float Y_MaxLimit = 80f;
    public float cullingHeight = 1f;
    public LayerMask cullingLayer = 1 << 0;
    public bool isCollider = false;
    public bool isShake = false;

    private float currentLerpCameraTime = 0.0f;
    public bool isLerpCamera = false;

    private ECameraView mCameraView = ECameraView.LockView;
    public ECameraView cameraView
    {
        get
        {
            return mCameraView;
        }
        set
        {
            mCameraView = value;
            //            if (mCameraView == ECameraView.LockView)
            //            {
            //                isLerpCamera = true;
            //            }
        }
    }

    //    private bool lockCamera = false;
    //    public bool isLockCamera
    //    {
    //        get
    //        {
    //            return lockCamera;
    //        }
    //        set
    //        {
    //            lockCamera = value;
    //
    //			if (lockCamera)
    //			{
    //				cameraView = ECameraView.LockView;
    //			} 
    //			else
    //			{
    //				cameraView = ECameraView.FreeView;
    //			}
    //        }
    //    }

    public bool EnableCamera
    {
        set
        {
            if (this.gameObject.activeInHierarchy != value)
            {
                this.gameObject.SetActive(value);
            }
        }
    }
    #endregion

    #region hide properties
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public Vector3 targetPos;
    [HideInInspector]
    public TPCameraListData CameraStateList;
    [HideInInspector]
    public int index;
    [HideInInspector]
    public Transform TargetLookAt;

    [HideInInspector]
    public TPCameraState currentState;
    [HideInInspector]
    public TPCameraState lerpState;
    [HideInInspector]
    public TPCameraState cacheLerpState;
    [HideInInspector]
    public Vector3 LookPoint;
    [HideInInspector]
    public float targetHeight;
    #endregion

    #region ShakeCamera
    [HideInInspector]
    public Camera mainCamera;
    public bool MainCameraEnable
    {
        get
        {
            return mainCamera.enabled;
        }
        set
        {
            mainCamera.clearFlags = CameraClearFlags.Skybox;
            mainCamera.backgroundColor = new Color(0f, 0f, 0f, 0.1f);
            mainCamera.enabled = value;
        }
    }


    #endregion

    static bool mIsOnJoystickGUI = false;
    public bool IsOnJoystickGUI
    {
        get { return mIsOnJoystickGUI; }
        set { mIsOnJoystickGUI = value; }
    }

    private float cacheX = 0.0f;

    [HideInInspector]
    public event CameraExtraMovementDelegate cameraExtraMovement = null;
    public delegate void CameraExtraMovementDelegate(TPCamera obj);

    public Transform AudioListenerTransform;

    void Awake()
    {
        Init();
    }

    void Init()
    {
#if UNITY_EDITOR
        X_MouseSensitivity = 3;
        S_MouseSensitivity = 0.05f;
#endif

        AudioListenerTransform = this.transform.parent.GetComponentInChildren<AudioListener>().transform;

        if (lerpState == null)
        {
            lerpState = new TPCameraState("lerp");
            cacheLerpState = new TPCameraState("lerp");
        }
        if (currentState == null)
        {
            currentState = new TPCameraState("");
        }

        if (CameraStateList == null)
        {
            CameraStateList = Resources.Load<TPCameraListData>("Data/Camera/TPCameraListData");
            if (CameraStateList == null)
            {
                Debug.LogError("TPCameraListData don't exist!!!");
                return;
            }
        }

        mainCamera = this.GetComponentInChildren<Camera>();

        TargetLookAt = new GameObject("targetLookAt").transform;
        //TargetLookAt.hideFlags = HideFlags.HideInHierarchy;
        TargetLookAt.position = Vector3.one;
        TargetLookAt.rotation = Quaternion.identity;
        DontDestroyOnLoad(TargetLookAt);
        DontDestroyOnLoad(this.gameObject);
        ChangeState("Lock2p5", false);
    }

    void LateUpdate()
    {
        if (TargetLookAt == null)
        {
            return;
        }

        if (isLerpCamera)
        {
            LerpCamera();
        }

        CameraMovement();
    }

    float tempDistance = 0;
    void CameraMovement()
    {
        if (isLerpState)
        {
            currentState.Slerp(lerpState, currentSmoothBetweenStateTime / SmoothBetweenState, cacheLerpState);

            if (currentSmoothBetweenStateTime >= SmoothBetweenState)
            {
                isLerpState = false;
                currentSmoothBetweenStateTime = 0.0f;
            }
            else
            {
                currentSmoothBetweenStateTime += Time.deltaTime;
            }
        }

        //get current camera direction
        var camDir = (currentState.forward * TargetLookAt.forward) + (currentState.right * TargetLookAt.right);
        camDir = camDir.normalized;
        tempDistance = distance;

        if (target == null)
            return;

        //calculate a target pos
        targetPos = Vector3.Slerp(targetPos, target.position, SmoothFollow * Time.deltaTime);
        var cPos = targetPos + new Vector3(0, targetHeight, 0);

        if (isCollider)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(cPos, camDir, out hitInfo, distance, cullingLayer))
            {
                var t = hitInfo.distance - 0.5f;
                t -= currentState.minDistance;
                t /= (distance - currentState.minDistance);

                targetHeight = Mathf.Lerp(cullingHeight, targetHeight, Mathf.Clamp(t, 0.0f, 1.0f));
                cPos = target.position + new Vector3(0, targetHeight, 0);
            }

#if UNITY_EDITOR
            Debug.DrawLine(cPos, transform.position, Color.red);
#endif

            if (Physics.Raycast(cPos, camDir, out hitInfo, distance + 0.2f, cullingLayer))
            {
                tempDistance = hitInfo.distance - 0.5f;
            }
        }

        var lookPoint = cPos;
        lookPoint += (TargetLookAt.right * Vector3.Dot(camDir * tempDistance, TargetLookAt.right));

        transform.position = cPos + (camDir * tempDistance);
        if (lockTarget != null)
        {
            Vector3 relativePos = lockTarget.position - transform.position;
            if (relativePos != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 3.5f * Time.deltaTime);
            }
        }
        else
        {
            transform.LookAt(cPos);
        }
        TargetLookAt.position = cPos;

        //Add smooth 
        Quaternion newRot = Quaternion.Euler(mouseY, mouseX, 0);
        TargetLookAt.rotation = Quaternion.Slerp(TargetLookAt.rotation, newRot, SmoothCameraRotation * Time.deltaTime);

        if (mainCamera != null && mainCamera.fieldOfView != currentState.fieldOfView)
        {
            mainCamera.fieldOfView = currentState.fieldOfView;
        }
        if (cameraExtraMovement != null)
        {
            cameraExtraMovement(this);
        }
    }

    public void SetDistance(float value)
    {
        if (mCameraView == ECameraView.LockView || mCameraView == ECameraView.Lock2P5View)
        {
            SetFreeViewDistance(value);
        }
        else
        {
            SetFreeViewDistance(value);
        }
    }


    /// <summary>
    /// 一次性设置摄像机的位置、朝向到位，不进行差值计算，用于场景加载完的一瞬间调用。
    /// </summary>
    public void SetCameraInfo()
    {
        currentState.Slerp(lerpState, 1, cacheLerpState);
        targetHeight = currentState.Height;

        mouseX = lerpState.mouseX;
        mouseY = lerpState.mouseY;
        distance = lerpState.distance;

        //get current camera direction
        var camDir = (currentState.forward * TargetLookAt.forward) + (currentState.right * TargetLookAt.right);
        camDir = camDir.normalized;

        if (target == null)
            return;

        //calculate a target pos
        targetPos = target.position;
        var cPos = targetPos + new Vector3(0, targetHeight, 0);

        if (isCollider)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(cPos, camDir, out hitInfo, distance, cullingLayer))
            {
                var t = hitInfo.distance - 0.2f;
                t -= currentState.minDistance;
                t /= (distance - currentState.minDistance);

                targetHeight = Mathf.Lerp(cullingHeight, targetHeight, Mathf.Clamp(t, 0.0f, 1.0f));
                cPos = target.position + new Vector3(0, targetHeight, 0);
            }

#if UNITY_EDITOR
            Debug.DrawLine(cPos, transform.position, Color.red);
#endif
            if (Physics.Raycast(cPos, camDir, out hitInfo, distance + 0.2f, cullingLayer))
            {
                distance = hitInfo.distance - 0.2f;
            }
        }
        transform.position = cPos + (camDir * distance);
        if (lockTarget != null)
        {
            Vector3 relativePos = lockTarget.position - transform.position;
            if (relativePos != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(relativePos);
            }
        }
        else
        {
            transform.LookAt(cPos);
        }
    }

    void SetLockViewDistance(float value)
    {
        if (value > 0)
        {
            if (distance >= lerpState.startZoomDistance && distance <= lerpState.maxDistance)
            {
                distance -= lerpState.zoomDistanceAate;
            }
            else if (distance >= lerpState.endZoomDistance && distance < lerpState.startZoomDistance)
            {
                mouseY -= lerpState.zoomAngleAateY;
                mouseX -= lerpState.zoomAngleAateX;
                distance -= lerpState.zoomDistanceAate;
            }
            else if (distance >= lerpState.minDistance && distance < lerpState.endZoomDistance)
            {
                distance -= lerpState.zoomDistanceAate;
            }
        }
        else
        {
            if (distance >= lerpState.startZoomDistance && distance <= lerpState.maxDistance)
            {
                distance += lerpState.zoomDistanceAate;
            }
            else if (distance >= lerpState.endZoomDistance && distance < lerpState.startZoomDistance)
            {
                mouseY += lerpState.zoomAngleAateY;
                mouseX += lerpState.zoomAngleAateX;
                distance += lerpState.zoomDistanceAate;
            }
            else if (distance >= lerpState.minDistance && distance < lerpState.endZoomDistance)
            {
                distance += lerpState.zoomDistanceAate;
            }
        }
        mouseX = Mathf.Clamp(mouseX, lerpState.endZoomAngle.x, lerpState.startZoomAngle.x);
        mouseY = Mathf.Clamp(mouseY, lerpState.endZoomAngle.y, lerpState.startZoomAngle.y);
        distance = Mathf.Clamp(distance, currentState.minDistance, currentState.maxDistance);
    }

    void SetFreeViewDistance(float value)
    {
        distance -= value * S_MouseSensitivity;
        distance = Mathf.Clamp(distance, currentState.minDistance, currentState.maxDistance);
    }

    public void SetRotation(Vector2 value)
    {
        if (mCameraView == ECameraView.LockView)
        {
            return;
        }

        isLerpCamera = false;

        if (mCameraView == ECameraView.Lock2P5View)
        {
            mouseX += (value.x * X_MouseSensitivity);
        }
        else
        {
            if (Mathf.Abs(value.x) > Mathf.Abs(value.y))
            {
                value.y = 0;
            }
            else
            {
                value.x = 0;
            }

            mouseX += (value.x * X_MouseSensitivity);
            mouseY -= (value.y * Y_MouseSensitivity);
            mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);
        }
    }

    private ECameraView tempCameraView;
    /// <summary>
    /// 震动摄像机
    /// </summary>
    /// <param name="force">震动幅度</param>
    /// <param name="duration">持续时间</param>
    public void Shake(float xForce, float yForce, float zForce, float duration)
    {
        if (mainCamera == null || mainCamera.Equals(null))
            return;

        //Action ac = new Action(onCameraShakeEnd);

        Hashtable ht = new Hashtable();
        ht.Add("time", duration);
        ht.Add("amount", new Vector3(xForce, yForce, zForce));
        Action action = onCameraShakeEnd;
        ht.Add("oncomplete", action); //(iTween.CompleteMethod)onCameraShakeEnd);
        ht.Add("islocal", true);
        iTween.ShakePosition(mainCamera.gameObject, ht);
        //iTween.ShakePosition(mainCamera.gameObject, new Vector3(xForce, yForce, zForce), duration);
        tempCameraView = cameraView;
        //cameraView = ECameraView.LockView;
    }

    private void onCameraShakeEnd()
    {
        if (mainCamera == null || mainCamera.Equals(null))
            return;

        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;
        //cameraView = tempCameraView;
    }

    public void ChangeToSurfaceState()
    {
        mouseX %= 360f;

        cacheX = mouseX;

        Vector3 cameraForward = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up);
        cameraForward.Normalize();
        cameraForward *= -1.0f;

        ChangeState("Surface", true);
        isLerpCamera = true;

        float arcAngle = Vector3.Dot(cameraForward, target.forward);
        arcAngle = Mathf.Clamp(arcAngle, -0.999f, 0.999f);
        float angle = Mathf.Rad2Deg * Mathf.Acos(arcAngle);

        if (angle.Equals(float.NaN))
        {
            angle = 0.0f;
        }

        Vector3 rotateForward = Vector3.Cross(cameraForward, target.forward);

        if (Vector3.Dot(rotateForward.normalized, Vector3.up) > 0)
        {
            lerpState.mouseX = (mouseX + angle);
        }
        else
        {
            lerpState.mouseX = mouseX + angle * -1f;
        }
    }

    public void ChangeFromSurfaceState()
    {
        mouseX %= 360f;

        if (cacheX > 0)
        {
            if (mouseX < 0)
            {
                mouseX += 360f;
            }
        }
        else
        {
            if (mouseX > 0)
            {
                mouseX -= 360f;
            }
        }

        ChangeState("Lock2p5", true);
        isLerpCamera = true;
        lerpState.mouseX = cacheX;
    }

    public void ChangeState(string stateName, bool hasSmooth)
    {
        var state = CameraStateList.tpCameraStates.Find(
            delegate (TPCameraState obj)
            {
                return obj.Name.Equals(stateName);
            }
        );

        if (state != null)
        {
            lerpState.CopyState(state);
            cacheLerpState.CopyState(state);

            if (currentState != null)
            {
                if (!hasSmooth)
                {
                    currentState.CopyState(state);
                }
                else
                {
                    isLerpState = true;
                    currentSmoothBetweenStateTime = 0.0f;
                }
            }
        }
        else
        {
            state = CameraStateList.tpCameraStates[0];
            lerpState.CopyState(state);
            cacheLerpState.CopyState(state);

            if (currentState != null)
            {
                if (!hasSmooth)
                {
                    currentState.CopyState(state);
                }
                else
                {
                    isLerpState = true;
                    currentSmoothBetweenStateTime = 0.0f;
                }
            }
        }

        //设置摄像机广角;
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = state.fieldOfView;
        }
        index = CameraStateList.tpCameraStates.IndexOf(state);

        cacheLerpState.mouseX = mouseX;
        cacheLerpState.mouseY = mouseY;
        cacheLerpState.distance = distance;
        cacheLerpState.Height = targetHeight;
        currentLerpCameraTime = 0.0f;
    }

    public void SetLockCamera(float angle)
    {
        if (!Mathf.Approximately(angle, -1f))
        {
            angle %= 360f;
            mCameraView = ECameraView.LockView;
            mouseX = angle;
        }
        else
        {
            mCameraView = ECameraView.Lock2P5View;
        }
    }

    public void SetCameraRotate(float angle)
    {
        angle %= 360f;
        mouseX = angle;
    }

    private void LerpCamera()
    {
        //int loop = 0;

        //        mouseX = (mouseX) % 360;
        //        if (mouseX < -180)
        //        {
        //            mouseX += 360;
        //        }
        //        else if (mouseX > 180)
        //        {
        //            mouseX -= 360;
        //        }

        //if (Mathf.Abs(mouseX - lerpState.mouseX) > 360)
        //      {
        //          loop = (int)(mouseX / 360) + (int)(mouseX / mouseX);
        //      }
        //      float target = lerpState.mouseX + loop * 360;
        mouseX = Mathf.Lerp(cacheLerpState.mouseX, lerpState.mouseX, currentLerpCameraTime / SmoothBetweenState);

        //Debug.LogError(mouseX);
        mouseY = Mathf.Lerp(cacheLerpState.mouseY, lerpState.mouseY, currentLerpCameraTime / SmoothBetweenState);
        distance = Mathf.Lerp(cacheLerpState.distance, lerpState.distance, currentLerpCameraTime / SmoothBetweenState);
        targetHeight = Mathf.Lerp(cacheLerpState.Height, lerpState.Height, currentLerpCameraTime / SmoothBetweenState);

        //		if (Mathf.Abs(mouseX - lerpState.mouseX) < 0.1f && 
        //			Mathf.Abs(mouseY - lerpState.mouseY) < 0.1f && 
        //			Mathf.Abs(distance - lerpState.distance) < 0.1f)
        //        {
        //            isLerpCamera = false;
        //        }

        if (currentLerpCameraTime >= SmoothBetweenState)
        {
            isLerpCamera = false;
            currentLerpCameraTime = 0.0f;
        }
        else
        {
            currentLerpCameraTime += Time.deltaTime;
        }
    }

    public void OnDestroy()
    {
        if (TargetLookAt != null)
        {
            GameObject.Destroy(TargetLookAt.gameObject);
            TargetLookAt = null;
        }
        else
        {
            Debug.LogWarning("OnDestroy : TPCamera TargetLookAt");
        }

        if (_instance != null)
        {
            GameObject.Destroy(_instance.gameObject);
            _instance = null;
        }
        else
        {
            Debug.LogWarning("OnDestroy : TPCamera _instance");
        }
    }

    //public void SetImageEffectParam(bool isUseColorCorrection, string colorCorrectionCurvesParams, string bloomParams)
    //{
    //	QualityPrototype qualityPrototype = PrototypeManager.Instance.GetPrototype<QualityPrototype>((int)SystemSettingManager.Instance.GraphicQualityType);

    //	UnityStandardAssets.ImageEffects.ColorCorrectionCurves colorCorrectionCurves = mainCamera.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>();
    //	colorCorrectionCurves.enabled = qualityPrototype.IsOpenImageEffects && isUseColorCorrection;
    //	//colorCorrectionCurves.SetParam(colorCorrectionCurvesParams);

    //	UnityStandardAssets.ImageEffects.BloomOptimized bloom = mainCamera.GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>();
    //	bloom.SetParam(bloomParams);
    //}

    public void SetFirstBattleCamera()
    {
        if (mainCamera == null || mainCamera.Equals(null))
            return;

        GameObject parent = new GameObject();

        parent.transform.parent = mainCamera.transform.parent;
        //		parent.transform.localPosition = new Vector3(-0.9587493f, 1.726917f, 15.28636f);
        //		parent.transform.localRotation = Quaternion.Euler(new Vector3(23.71591f, 180f, 0f));

        parent.transform.position = new Vector3(82.42f, 26.19f, 151.21f);
        parent.transform.rotation = Quaternion.Euler(new Vector3(-8.284119f, 180f, 0f));

        mainCamera.transform.parent = parent.transform;
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;
    }

    public void RevertFirstBattleCamera()
    {
        if (mainCamera == null || mainCamera.Equals(null))
            return;

        GameObject cameraRoot = mainCamera.transform.parent.gameObject;

        mainCamera.transform.parent = cameraRoot.transform.parent;
        iTween.MoveTo(mainCamera.gameObject,
            iTween.Hash("position", Vector3.zero, "islocal", true, "time", 1f, "easetype", iTween.EaseType.linear));

        iTween.RotateTo(mainCamera.gameObject,
            iTween.Hash("rotation", Vector3.zero, "islocal", true, "time", 1f, "easetype", iTween.EaseType.linear));

        GameObject.Destroy(cameraRoot);
    }
}

public static class ExtensionMethods
{
    public static void Slerp(this TPCameraState to, TPCameraState from, float time, TPCameraState cacha)
    {
        to.forward = Mathf.Lerp(cacha.forward, from.forward, time);
        to.right = Mathf.Lerp(cacha.right, from.right, time);
        to.minDistance = Mathf.Lerp(cacha.minDistance, from.minDistance, time);
        to.maxDistance = Mathf.Lerp(cacha.maxDistance, from.maxDistance, time);
        to.Height = Mathf.Lerp(cacha.Height, from.Height, time);
    }

    public static void CopyState(this TPCameraState to, TPCameraState from)
    {
        to.forward = from.forward;
        to.right = from.right;
        to.mouseX = from.mouseX;
        to.mouseY = from.mouseY;
        to.distance = from.distance;
        to.fieldOfView = from.fieldOfView;
        to.maxDistance = from.maxDistance;
        to.minDistance = from.minDistance;
        to.Height = from.Height;
        to.startZoomAngle = from.startZoomAngle;
        to.endZoomAngle = from.endZoomAngle;
        to.zoomAngleAateX = from.zoomAngleAateX;
        to.zoomAngleAateY = from.zoomAngleAateY;
        to.startZoomDistance = from.startZoomDistance;
        to.endZoomDistance = from.endZoomDistance;
        to.zoomDistanceAate = from.zoomDistanceAate;
    }
}

public static class Helper
{
    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        do
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
        } while (angle < -360 || angle > 360);

        return Mathf.Clamp(angle, min, max);
    }

    public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
    {
        var clipPlanePoints = new ClipPlanePoints();

        if (Camera.main == null)
            return clipPlanePoints;

        var transform = Camera.main.transform;
        var halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
        var aspect = Camera.main.aspect;
        var distance = Camera.main.nearClipPlane;
        var height = distance * Mathf.Tan(halfFOV);
        var width = height * aspect;

        clipPlanePoints.LowerRight = pos + transform.right * width;
        clipPlanePoints.LowerRight -= transform.up * height;
        clipPlanePoints.LowerRight += transform.forward * distance;

        clipPlanePoints.LowerLeft = pos - transform.right * width;
        clipPlanePoints.LowerLeft -= transform.up * height;
        clipPlanePoints.LowerLeft += transform.forward * distance;

        clipPlanePoints.UpperRight = pos + transform.right * width;
        clipPlanePoints.UpperRight += transform.up * height;
        clipPlanePoints.UpperRight += transform.forward * distance;

        clipPlanePoints.UpperLeft = pos - transform.right * width;
        clipPlanePoints.UpperLeft += transform.up * height;
        clipPlanePoints.UpperLeft += transform.forward * distance;

        return clipPlanePoints;
    }
}