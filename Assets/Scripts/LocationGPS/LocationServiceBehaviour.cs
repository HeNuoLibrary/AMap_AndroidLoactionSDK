using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class LocationServiceBehaviour : MonoBehaviour
{
    [SerializeField] Text Location;
    [SerializeField] Button BtnGotolocal;

    public bool IsGPS { get { return Input.location.status == LocationServiceStatus.Running; } }

    public LocationInfo LocationData
    {
        get
        {
            return Input.location.lastData;
        }
    }

    public string GetLocationInfo
    {
        get
        {
            string latitude = $"纬度Location:{Input.location.lastData.latitude}\n";
            string longitude = $"经度longitude:{Input.location.lastData.longitude}\n";
            string altitude = $"高度altitude:{Input.location.lastData.altitude}\n";
            string horizontalAccuracy = $"水平精确度horizontalAccuracy:{Input.location.lastData.horizontalAccuracy}\n";
            string timestamp = $"时间戳timestamp:{Input.location.lastData.timestamp}\n";

            return latitude + longitude + altitude + horizontalAccuracy + timestamp;
        }
    }

    /// <summary>
    /// 启动定位服务
    /// </summary>
    /// <param name="desiredAccuracyInMeters">服务所需的精度，以米为单位。如果使用较高的值，比如500，那么通常不需要打开GPS芯片（比如可以利用信号基站进行三角定位），从而节省电池电量。像5-10这样的值，可以被用来获得最佳的精度。默认值是10米。</param>
    /// <param name="updateDistanceInMeters">最小距离（以米为单位）的设备必须横向移动前Input.location属性被更新。较高的值，如500意味着更少的开销。默认值是10米。</param>
    /// <returns></returns>
    public void StartLocationService(float desiredAccuracyInMeters,float updateDistanceInMeters)
    {
        //（1）isEnabledByUser   -- 用户设置里的定位服务是否启用。（实测发现，都为true，似乎不大起作用）
        //（2）lastData-- 最近一次测量的地理位置（LocationInfo lastData； 也就是要和 LocationInfo 关联了）
        //（3）status-- 定位服务的状态。
        if (!Input.location.isEnabledByUser) return;
        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
    }

    private IEnumerator LocationServiceStart(float desiredAccuracyInMeters, float updateDistanceInMeters)
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("location 权限未开");
            yield break;
        }

        // Starts the location service.
        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.LogError("location 启动时间超时");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("location 启动失败");
            yield break;
        }

        Gotolocation();
    }

    /// <summary>
    /// 停止定位服务的定位更新。这对节省电池的电量非常有用
    /// </summary>
    public void StopLocationService()
    {
        Input.location.Stop();
    }

    private void Start()
    {
        StartCoroutine(LocationServiceStart(5f, 5f));

        BtnGotolocal.onClick.AddListener(Gotolocation);
    }

    private void Update()
    {
        if (IsGPS && Location != null)
        {
            Location.text = GetLocationInfo;
        }
    }

    private void OnDestroy()
    {
        StopLocationService();
    }

    // 移动到定位中心
    public void Gotolocation()
    {
        if (IsGPS)
        {
            var longitude = Input.location.lastData.longitude;
            var latitude = Input.location.lastData.latitude;
            if (GpsUtil.UserGD)
            {
                var location = GpsUtil.Gps84ToGcj02(latitude, longitude);

                Vector2 position = new Vector2 { x = (float)location[1], y = (float)location[0] };
                //Map.floatZoom = 16;
                //Map.position = position;
            }
            else
            {
                Vector2 position = new Vector2 { x = longitude, y = latitude };
                //Map.floatZoom = 16;
                //Map.position = position;
            }
        }
    }
}

