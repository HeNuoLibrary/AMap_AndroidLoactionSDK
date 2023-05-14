using AMapLocationHelper;
using UnityEngine;
using System;
using TMPro;

public class AMapDemo : MonoBehaviour
{
	public TMP_Text txt;
	private AMapLocation location;

	private void Start()
	{
		location = GetComponent<AMapLocation>();
		ShowTxt("scene start");
		ShowTxt(location.ToString());
	}

	public void StartLocation()
	{
		ShowTxt("start");
		location.StartLocation();
		location.locationChanged += OnLocationChanged;
	}

	public void EndLocation()
	{
		ShowTxt("end");
		location.locationChanged -= OnLocationChanged;
		location.EndLocation();
	}

	private void OnLocationChanged()
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

	///// <summary>
	///// 显示位置信息
	///// </summary>
 //   public void ShowMapInfo()
 //   {
 //       StartCoroutine(GetMap());
 //   }
	///// <summary>
	///// 获取地图信息
	///// </summary>
	///// <returns></returns>
 //   IEnumerator GetMap()
 //   {
 //       double a = Math.Round(location.locationInfo.Longitude, 6);
 //       double b = Math.Round(location.locationInfo.Latitude, 6);
	//	string url = $"https://restapi.amap.com/v3/staticmap?location={a},{b}&zoom={Slider.value}&size=1024*1024&markers=mid,,A:116.481485,39.990464&key=5a2c92b8a10471ba29a810b413d5742a";
 //       //url = "https://restapi.amap.com/v3/staticmap?location=116.481485,39.990464&zoom=13&size=1024*1024&markers=mid,,A:116.481485,39.990464&key=5a2c92b8a10471ba29a810b413d5742a";
 //       DownloadHandlerTexture downloadHandlerTexture= new DownloadHandlerTexture();//
 //       UnityWebRequest uwp=UnityWebRequestTexture.GetTexture(url);
 //       yield return uwp.SendWebRequest();
 //       if (uwp.isNetworkError || uwp.isHttpError)
 //       {
 //           Debug.Log(uwp.error);
 //       }
 //       else
 //       {
 //           Texture2D myTexture = ((DownloadHandlerTexture)uwp.downloadHandler).texture;
 //           Sprite createSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
 //           mapTransform.GetComponent<Image>().sprite = createSprite;
 //           mapTransform.GetComponent<Image>().SetNativeSize();
 //       }
 //   }
}