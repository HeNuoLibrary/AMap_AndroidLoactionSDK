# 高德Android定位SDK for Unity

# Unity自带的定位功能

```C#
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
```

## 高德Android定位SDK

### 1.申请高德定位 Key

- 获取 keystore 里面的SHA1 数据

  参考链接 : [Unity KeyStore创建并且获取MD5,SHA1,SHA256数据_unity 获取sha1_倒转六年的博客-CSDN博客](https://blog.csdn.net/qq_34263160/article/details/126306104)

  keytool.exe 路径 D:\Applitation\UnityEditor\2021.3.23f1c1\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK\bin

  例如 : 

  ![](C:\Users\HeNuo\Desktop\AMap_AndroidSDK\MDImages\c6022ce4-5026-4782-8f91-9b629f898cfb.png)

- 在高德控制台中添加key

  通过上一步 获取的 SHA1 和 应用程序的包名(例如:com.xxx.xxx) 创建高德的key

  [参考链接:  获取Key-创建工程-开发指南-Android 定位SDK | 高德地图API](https://lbs.amap.com/api/android-location-sdk/guide/create-project/get-key)

### 2.制作高德定位插件jar包

- 下载高德的Android定位SDK 下载链接 : [相关下载-Android 定位SDK | 高德地图API](https://lbs.amap.com/api/android-location-sdk/download)

- 创建AndroidStudio工程 将下载的Android定位SDK导入制作AMapLocationModeHelper助手

  ![](C:\Users\HeNuo\Desktop\AMap_AndroidSDK\MDImages\1280X1280.PNG)

AMapLocationModeHelper 具体内容

```C#
package com.xzp.u2a;
import com.amap.api.location.AMapLocationClientOption;
import com.amap.api.location.AMapLocationClientOption.AMapLocationMode;
import com.amap.api.location.AMapLocationClient;
public class AMapLocationModeHelper {
    public void hightAccuracy(AMapLocationClientOption mLocationOption)
    {
        //初始化定位
       
        mLocationOption.setLocationMode(AMapLocationMode.Hight_Accuracy);
    }
    public void batterySaving(AMapLocationClientOption mLocationOption)
    {
        mLocationOption.setLocationMode(AMapLocationMode.Battery_Saving);
    }
    public void deviceSensors(AMapLocationClientOption mLocationOption)
    {
        mLocationOption.setLocationMode(AMapLocationMode.Device_Sensors);
    }
    public void onceLocation(AMapLocationClientOption mLocationOption)
    {
        mLocationOption.setOnceLocation(true);
    }
    public void onceLocationLatest(AMapLocationClientOption mLocationOption)
    {
        mLocationOption.setOnceLocationLatest(true);
    }
    public void interval(AMapLocationClientOption mLocationOption,int time)
    {
        mLocationOption.setInterval(time);
    }
    public void wifiActiveScan(AMapLocationClientOption mLocationOption)
    {
        mLocationOption.setWifiScan(false);
    }
    public void mockEnable(AMapLocationClientOption mLocationOption)
    {
        mLocationOption.setMockEnable(false);
    }
    public void needAddress(AMapLocationClientOption mLocationOption){
        mLocationOption.setNeedAddress(false);
    }
}
```

## 3.导入Unity进行配置

- 插件导入unity 将上一步得到的 jar包和高德Android定位SDK导入到unity中

  ![](C:\Users\HeNuo\Desktop\AMap_AndroidSDK\MDImages\e95b6229-ca48-4569-874d-b5bf842c584f.png)

- 配置AndroidManifest.xml文件 参考 : [获取定位数据-获取位置-开发指南-Android 定位SDK | 高德地图API](https://lbs.amap.com/api/android-location-sdk/guide/android-location/getlocation)

  必填的配置

  ```C#
  package="com.xxx.xxx"> 要保持和申请Key的保持一致
  <service android:name="com.amap.api.location.APSService"></service>
  <meta-data android:name="com.amap.api.v2.apikey" android:value="开发者申请的key ">         
  </meta-data>
  
  <!--用于进行网络定位-->
  <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION"></uses-permission>
  <!--用于访问GPS定位-->
  <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"></uses-permission>
  <!--用于获取运营商信息，用于支持提供运营商信息相关的接口-->
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"></uses-permission>
  <!--用于访问wifi网络信息，wifi信息会用于进行网络定位-->
  <uses-permission android:name="android.permission.ACCESS_WIFI_STATE"></uses-permission>
  <!--用于获取wifi的获取权限，wifi信息会用来进行网络定位-->
  <uses-permission android:name="android.permission.CHANGE_WIFI_STATE"></uses-permission>
  <!--用于访问网络，网络定位需要上网-->
  <uses-permission android:name="android.permission.INTERNET"></uses-permission>
  <!--用于读取手机当前的状态-->
  <uses-permission android:name="android.permission.READ_PHONE_STATE"></uses-permission>
  <!--用于写入缓存数据到扩展存储卡-->
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE"></uses-permission>
  <!--用于申请调用A-GPS模块-->
  <uses-permission android:name="android.permission.ACCESS_LOCATION_EXTRA_COMMANDS"></uses-permission>
  <!--如果设置了target >= 28 如果需要启动后台定位则必须声明这个权限-->
  <uses-permission android:name="android.permission.FOREGROUND_SERVICE"/>
  <!--如果您的应用需要后台定位权限，且有可能运行在Android Q设备上,并且设置了target>28，必须增加这个权限声明-->
  <uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION"/>
  ```

完整的AndroidManifest.xml

```C#
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<manifest 
  xmlns:android="http://schemas.android.com/apk/res/android" 
  android:installLocation="preferExternal" 
  package="com.xxx.xxx">
        <supports-screens 
                                android:anyDensity="true" 
                                android:largeScreens="true" 
                                android:normalScreens="true" 
                                android:smallScreens="true" 
                                android:xlargeScreens="true"/>
        <application 
                                android:debuggable="false" 
                                android:isGame="true" 
        android:icon="@drawable/app_icon"
        android:label="@string/app_name"
                                android:theme="@style/UnityThemeSelector">
    <!-- 请在application标签中声明service组件,每个app拥有自己单独的定位service。 -->
                <service android:name="com.amap.api.location.APSService"></service>
    <!-- 设置高德的Key -->
                <meta-data android:name="com.amap.api.v2.apikey" android:value="高德地图Key"></meta-data>
                
    <activity android:configChanges="locale|fontScale|keyboard|keyboardHidden|mcc|mnc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|touchscreen|uiMode" 
              android:label="@string/app_name" 
              android:launchMode="singleTask" 
              android:name="com.unity3d.player.UnityPlayerActivity" 
              android:screenOrientation="portrait">
                        <intent-filter>
                                <action android:name="android.intent.action.MAIN"/>
                                <category android:name="android.intent.category.LAUNCHER"/>
                                <category android:name="android.intent.category.LEANBACK_LAUNCHER"/>
                        </intent-filter>
                        <meta-data android:name="unityplayer.UnityActivity" android:value="true"/>
                </activity>
        </application>
        <uses-feature android:glEsVersion="0x20000"/>
        <uses-feature android:name="android.hardware.touchscreen" android:required="false"/>
        <uses-feature android:name="android.hardware.touchscreen.multitouch" android:required="false"/>
        <uses-feature android:name="android.hardware.touchscreen.multitouch.distinct" android:required="false"/>
  
  
  <!--用于进行网络定位-->
  <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION"></uses-permission>
  <!--用于访问GPS定位-->
  <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"></uses-permission>
  <!--用于获取运营商信息，用于支持提供运营商信息相关的接口-->
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"></uses-permission>
  <!--用于访问wifi网络信息，wifi信息会用于进行网络定位-->
  <uses-permission android:name="android.permission.ACCESS_WIFI_STATE"></uses-permission>
  <!--用于获取wifi的获取权限，wifi信息会用来进行网络定位-->
  <uses-permission android:name="android.permission.CHANGE_WIFI_STATE"></uses-permission>
  <!--用于访问网络，网络定位需要上网-->
  <uses-permission android:name="android.permission.INTERNET"></uses-permission>
  <!--用于读取手机当前的状态-->
  <uses-permission android:name="android.permission.READ_PHONE_STATE"></uses-permission>
  <!--用于写入缓存数据到扩展存储卡-->
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE"></uses-permission>
  <!--用于申请调用A-GPS模块-->
  <uses-permission android:name="android.permission.ACCESS_LOCATION_EXTRA_COMMANDS"></uses-permission>
  <!--如果设置了target >= 28 如果需要启动后台定位则必须声明这个权限-->
  <uses-permission android:name="android.permission.FOREGROUND_SERVICE"></uses-permission>
  <!--如果您的应用需要后台定位权限，且有可能运行在Android Q设备上,并且设置了target>28，必须增加这个权限声明-->
  <uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION"></uses-permission>
  
</manifest>
```

- 编写调用代码

```C#
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class AMapDemo : MonoBehaviour
{

        private AMapLocationHelper.AMapLocation location;
        public Text txt;

        void Start()
        {
                location = GetComponent<AMapLocationHelper.AMapLocation>();
                ShowTxt("scene start");
                ShowTxt(location.ToString());
        }

        public void startLocation()
        {
                ShowTxt("start");
                location.StartLocation();
                location.locationChanged += OnLocationChanged;
        }

        public void endLocation()
        {
                ShowTxt("end");
                location.locationChanged -= OnLocationChanged;
                location.EndLocation();
        }

        void OnLocationChanged()
        {
                txt.text = "";
                ShowTxt("OnLocationChanged");
                if (!location.error)
                {
                        try
                        {
                                ShowTxt("定位结果来源：" + location.locationInfo.Accuracy);
                                ShowTxt("纬度：" + location.locationInfo.Latitude.ToString());
                                ShowTxt("经度：" + location.locationInfo.Longitude.ToString());
                                ShowTxt("精度信息：" + location.locationInfo.Accuracy.ToString());
                                ShowTxt("地址：" + location.locationInfo.Address);
                                ShowTxt("城区：" + location.locationInfo.District);
                                ShowTxt("国家：" + location.locationInfo.Country);
                                ShowTxt("省：" + location.locationInfo.Province);
                                ShowTxt("城区：" + location.locationInfo.City);
                                ShowTxt("街道：" + location.locationInfo.Street);
                                ShowTxt("门牌：" + location.locationInfo.StreetNum);
                                ShowTxt("城市编码：" + location.locationInfo.CityCode);
                                ShowTxt("地区编码：" + location.locationInfo.AdCode);
                                ShowTxt("海拔：" + location.locationInfo.Altitude.ToString());
                                ShowTxt("方向角：" + location.locationInfo.Bearing.ToString());
                                ShowTxt("定位信息描述：" + location.locationInfo.LocationDetail);
                                ShowTxt("兴趣点：" + location.locationInfo.PoiName);
                                ShowTxt("提供者：" + location.locationInfo.Provider);
                                ShowTxt("卫星数量：" + location.locationInfo.Satellites.ToString());
                                ShowTxt("当前速度：" + location.locationInfo.Speed.ToString());
                                ShowTxt("时间：" + location.locationInfo.Time.ToLongTimeString());
                        }
                        catch (Exception ex)
                        {
                                ShowTxt(ex.Message);
                        }
                }
                else
                {
                        ShowTxt(location.errorInfo);
                }
        }

        private void ShowTxt(string info)
        {
                txt.text = info + "\r\n" + txt.text;
        }
}
```

```C#
using UnityEngine;
using System;

namespace AMapLocationHelper
{

        public class AMapLocation : MonoBehaviour
        {
                private AMapLocationEvent amapEvent;
                private AndroidJavaClass jcu;
                private AndroidJavaObject jou;
                private AndroidJavaObject mLocationClient;
                private AndroidJavaObject mLocationOption;

                /// <summary>
                /// 定位模式
                /// </summary>
                public LocationMode locationMode = LocationMode.HightAccuracy;
                /// <summary>
                /// 获取一次定位结果
                /// </summary>
                public bool onceLocation = false;
                /// <summary>
                /// 获取最近3s内精度最高的一次定位结果：
                /// </summary>
                public bool onceLocationLatest = false;
                /// <summary>
                /// 定位间隔,单位毫秒
                /// </summary>
                public int interval = 2000;
                /// <summary>
                /// 是否返回地址信息
                /// </summary>
                public bool needAddress = true;
                /// <summary>
                /// 是否强制刷新WIFI
                /// </summary>
                public bool wifiActiveScan = true;
                /// <summary>
                /// 是否允许模拟软件Mock位置结果
                /// </summary>
                public bool mockEnable = false;
                /// <summary>
                /// 错误
                /// </summary>
                [HideInInspector]
                public bool error = false;
                /// <summary>
                /// 错误信息
                /// </summary>
                [HideInInspector]
                public string errorInfo = "";
                /// <summary>
                /// 定位结果信息
                /// </summary>
                public AMapLocationInfo locationInfo = new AMapLocationInfo();

                public delegate void OnLocationChangedEvent();
                public event OnLocationChangedEvent locationChanged;

                /// <summary>
                /// 开始定位
                /// </summary>
                public void StartLocation()
                {
                        error = false;
                        errorInfo = "";

                        try
                        {
                                jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                                jou = jcu.GetStatic<AndroidJavaObject>("currentActivity");

                                #region 初始化定位 
                                //声明AMapLocationClient类对象
                                mLocationClient = null;

                                //声明定位回调监听器
                                amapEvent = new AMapLocationEvent();
                                amapEvent.locationChanged += OnLocationChanged;

                                try
                {
                    AndroidJavaClass aClient = new AndroidJavaClass("com.amap.api.location.AMapLocationClient");
                    aClient.CallStatic("updatePrivacyShow", jou, true, true);
                    aClient.CallStatic("updatePrivacyAgree", jou, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                                //初始化定位
                                mLocationClient = new AndroidJavaObject("com.amap.api.location.AMapLocationClient", jou);

              
            
                //设置定位回调监听
                                mLocationClient.Call("setLocationListener", amapEvent);
                                #endregion

                                #region 配置参数
                                //声明AMapLocationClientOption对象
                                //public AMapLocationClientOption mLocationOption = null;
                                //初始化AMapLocationClientOption对象
                                //mLocationOption = new AMapLocationClientOption();
                                mLocationOption = null;
                                mLocationOption = new AndroidJavaObject("com.amap.api.location.AMapLocationClientOption");
                #endregion

                #region 设置定位配置
                // AndroidPlugin插件声明一个实例 进行调用(当时开发时的包名)
                AndroidJavaObject helper = new AndroidJavaObject("com.xzp.u2a.AMapLocationModeHelper");

                                //选择定位模式
                                switch (locationMode)
                                {
                                        case LocationMode.BatterSaving:
                                                helper.Call("batterySaving", mLocationOption);
                                                break;
                                        case LocationMode.DeviceSensors:
                                                helper.Call("deviceSensors", mLocationOption);
                                                break;
                                        case LocationMode.HightAccuracy:
                                                helper.Call("hightAccuracy", mLocationOption);
                                                break;
                                }

                                if (onceLocation)
                                {
                                        helper.Call("onceLocation", mLocationOption);
                                }

                                if (onceLocationLatest)
                                {
                                        helper.Call("onceLocationLatest", mLocationOption);
                                }

                                if (interval > 1000)
                                {
                                        helper.Call("interval", mLocationOption, interval);
                                }

                                if (!needAddress)
                                {
                                        helper.Call("needAddress", mLocationOption);
                                }

                                if (!wifiActiveScan)
                                {
                                        helper.Call("wifiActiveScan", mLocationOption);
                                }

                                if (mockEnable)
                                {
                                        helper.Call("mockEnable", mLocationOption);
                                }
                                #endregion

                                #region 启动定位
                                //给定位客户端对象设置定位参数
                                //mLocationClient.setLocationOption(mLocationOption);
                                //启动定位
                                //mLocationClient.startLocation();
                                mLocationClient.Call("setLocationOption", mLocationOption);
                                mLocationClient.Call("startLocation");
                                #endregion

                        }
                        catch (Exception ex)
                        {
                                Debug.Log(ex.Message);
                                error = true;
                                errorInfo = ex.Message;
                        }
                }

                /// <summary>
                /// 结束定位
                /// </summary>
                public void EndLocation()
                {
                        if (amapEvent != null)
                        {
                                amapEvent.locationChanged -= OnLocationChanged;
                        }

                        if (mLocationClient != null)
                        {
                                mLocationClient.Call("stopLocation");
                                mLocationClient.Call("onDestroy");
                        }

                        error = false;
                        errorInfo = "";
                }

                /// <summary>
                /// 定位事件
                /// </summary>
                /// <param name="amapLocation">AMap location.</param>
                private void OnLocationChanged(AndroidJavaObject amapLocation)
                {
                        if (amapLocation != null)
                        {
                                if (amapLocation.Call<int>("getErrorCode") == 0)
                                {
                                        try
                                        {
                                                locationInfo.LocationType = amapLocation.Call<int>("getLocationType");
                                                locationInfo.Latitude = amapLocation.Call<double>("getLatitude");
                                                locationInfo.Longitude = amapLocation.Call<double>("getLongitude");
                                                locationInfo.Accuracy = amapLocation.Call<float>("getAccuracy");
                                                locationInfo.Address = amapLocation.Call<string>("getAddress");
                                                locationInfo.Country = amapLocation.Call<string>("getCountry");
                                                locationInfo.Province = amapLocation.Call<string>("getProvince");
                                                locationInfo.City = amapLocation.Call<string>("getCity");
                                                locationInfo.District = amapLocation.Call<string>("getDistrict");
                                                locationInfo.Street = amapLocation.Call<string>("getStreet");
                                                locationInfo.StreetNum = amapLocation.Call<string>("getStreetNum");
                                                locationInfo.CityCode = amapLocation.Call<string>("getCityCode");
                                                locationInfo.AdCode = amapLocation.Call<string>("getAdCode");
                                                locationInfo.Altitude = amapLocation.Call<double>("getAltitude");
                                                locationInfo.Bearing = amapLocation.Call<float>("getBearing");
                                                locationInfo.LocationDetail = amapLocation.Call<string>("getLocationDetail");
                                                locationInfo.PoiName = amapLocation.Call<string>("getPoiName");
                                                locationInfo.Provider = amapLocation.Call<string>("getProvider");
                                                locationInfo.Satellites = amapLocation.Call<int>("getSatellites");
                                                locationInfo.AoiName = amapLocation.Call<string>("getAoiName");
                                                locationInfo.Speed = amapLocation.Call<float>("getSpeed");
                                                locationInfo.Time = DateTime.Now;
                                        }
                                        catch (Exception ex)
                                        {
                                                Debug.Log(ex.Message);
                                                error = true;
                                                errorInfo = ex.Message;
                                        }

                                }
                                else
                                {
                                        error = true;
                                        errorInfo = ">>getErrorCode:" + amapLocation.Call<int>("getErrorCode").ToString()
                                        + ">>getErrorInfo:" + amapLocation.Call<string>("getErrorInfo");
                                }
                        }
                        else
                        {
                                error = true;
                                errorInfo = "amaplocation is null.";
                        }

                        if (locationChanged != null)
                        {
                                locationChanged();
                        }
                }
        }

        /// <summary>
        /// 定位模式枚举
        /// </summary>
        public enum LocationMode
        {
                /// <summary>
                /// 高精度定位模式
                /// </summary>
                HightAccuracy,
                /// <summary>
                /// 低功耗定位模式
                /// </summary>
                BatterSaving,
                /// <summary>
                /// 仅用设备定位模式
                /// </summary>
                DeviceSensors
        }
}
```

```C#
using UnityEngine;

namespace AMapLocationHelper
{
    public class AMapLocationEvent : AndroidJavaProxy
    {

        public AMapLocationEvent() : base("com.amap.api.location.AMapLocationListener")
        {
        }

        void onLocationChanged(AndroidJavaObject amapLocation)
        {
            if (locationChanged != null)
            {
                locationChanged(amapLocation);
            }
        }

        public delegate void DelegateOnLocationChanged(AndroidJavaObject amap);
        public event DelegateOnLocationChanged locationChanged;
    }
}
```

```C#
using System;

namespace AMapLocationHelper
{
    /// <summary>
    /// 定位结果信息
    /// </summary>
    public class AMapLocationInfo
    {
        /// <summary>
        /// 定位结果来源:
        /// </summary>
        public int LocationType;
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude;
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude;
        /// <summary>
        /// 精度信息
        /// </summary>
        public float Accuracy;
        /// <summary>
        /// 地址
        /// </summary>
        public string Address;
        /// <summary>
        /// 城区
        /// </summary>
        public string District;
        /// <summary>
        /// 国家
        /// </summary>
        public string Country;
        /// <summary>
        /// 省
        /// </summary>
        public string Province;
        /// <summary>
        /// 城区
        /// </summary>
        public string City;
        /// <summary>
        /// 街道
        /// </summary>
        public string Street;
        /// <summary>
        /// 门牌
        /// </summary>
        public string StreetNum;
        /// <summary>
        /// 城市编码
        /// </summary>
        public string CityCode;
        /// <summary>
        /// 地区编码
        /// </summary>
        public string AdCode;
        /// <summary>
        /// 海拔
        /// </summary>
        public double Altitude;
        /// <summary>
        /// 方向角
        /// </summary>
        public float Bearing;
        /// <summary>
        /// 定位信息描述:
        /// </summary>
        public string LocationDetail;
        /// <summary>
        /// 兴趣点
        /// </summary>
        public string PoiName;
        /// <summary>
        /// 提供者
        /// </summary>
        public string Provider;
        /// <summary>
        /// 卫星数量:
        /// </summary>
        public int Satellites;
        /// <summary>
        /// aoi name
        /// </summary>
        public string AoiName;
        /// <summary>
        /// 当前速度
        /// </summary>
        public float Speed;
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time;
    }
}
```

- 打包测试

  注意事项:

  1. 打包项目的包名com.xxx.xxx 和 高德地图Key 保持一致
  2. 打包的keystore 要使用之前制作的 并填写密码
  3. 勾选 CustomMainManifest 使用配置的AndroidManifest.xml文件
  4. 打包运行后需要手动开启应用所需的权限

# GitHub仓库参考资源(感谢大神分享)

- [GitHub - xzp3094997376/Gd_map: 高德地图定位Android配置](https://github.com/xzp3094997376/Gd_map)(master分支)
- [GitHub - xzp3094997376/GdMap: Unity接入高德地图sdk_定位](https://github.com/xzp3094997376/GdMap)
- [GitHub - xzp3094997376/-map_UnityAndroid: 这是高德地图嵌入到android](https://github.com/xzp3094997376/-map_UnityAndroid)

# 参考文章总结

- [Unity KeyStore创建并且获取MD5,SHA1,SHA256数据_unity 获取sha1_倒转六年的博客-CSDN博客](https://blog.csdn.net/qq_34263160/article/details/126306104)
- [android打包jar包给unity使用&& 接入高德sdk，实现定位。_wodownload2的博客-CSDN博客](https://blog.csdn.net/wodownload2/article/details/106794074)
- [如何导出android studio程序,Android Studio 如何导出 Jar 给 Unity 使用_小状师张的博客-CSDN博客](https://blog.csdn.net/weixin_31461519/article/details/117510273)
- [全网最全最细Android Studio 安装和使用教程](https://zhuanlan.zhihu.com/p/528196912)
- [概述-Android 定位SDK | 高德地图API](https://lbs.amap.com/api/android-location-sdk/locationsummary)