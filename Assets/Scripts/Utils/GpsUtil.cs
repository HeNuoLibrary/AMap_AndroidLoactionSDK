using System;

// GPS、谷歌、百度、高德坐标相互转换
// https://blog.csdn.net/bopisky/article/details/80756322
public class GpsUtil
{
    /// <summary>
    /// 是否使用高德定位
    /// </summary>
    public static bool UserGD = false;
    public static double pi = 3.1415926535897932384626;
    public static double x_pi = 3.14159265358979324 * 3000.0 / 180.0;
    public static double a = 6378245.0;
    public static double ee = 0.00669342162296594323;

    public static double TransformLat(double x, double y)
    {
        double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
        ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
        ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
        return ret;
    }

    public static double TransformLon(double x, double y)
    {
        double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
        ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
        ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
        return ret;
    }

    public static double[] transform(double lat, double lon)
    {
        if (OutOfChina(lat, lon)) return new double[] { lat, lon };

        double dLat = TransformLat(lon - 105.0, lat - 35.0);
        double dLon = TransformLon(lon - 105.0, lat - 35.0);
        double radLat = lat / 180.0 * pi;
        double magic = Math.Sin(radLat);
        magic = 1 - ee * magic * magic;
        double SqrtMagic = Math.Sqrt(magic);
        dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * SqrtMagic) * pi);
        dLon = (dLon * 180.0) / (a / SqrtMagic * Math.Cos(radLat) * pi);
        double mgLat = lat + dLat;
        double mgLon = lon + dLon;
        return new double[] { mgLat, mgLon };
    }

    public static bool OutOfChina(double lat, double lon)
    {
        if (lon < 72.004 || lon > 137.8347) return true;
        if (lat < 0.8293 || lat > 55.8271)return true;
        return false;
    }

    /// <summary>
    /// GPS 转 高德坐标
    /// </summary>
    public static double[] Gps84ToGcj02(double lat, double lon)
    {
        if (OutOfChina(lat, lon)) return new double[] { lat, lon };

        double dLat = TransformLat(lon - 105.0, lat - 35.0);
        double dLon = TransformLon(lon - 105.0, lat - 35.0);
        double radLat = lat / 180.0 * pi;
        double magic = Math.Sin(radLat);
        magic = 1 - ee * magic * magic;
        double SqrtMagic = Math.Sqrt(magic);
        dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * SqrtMagic) * pi);
        dLon = (dLon * 180.0) / (a / SqrtMagic * Math.Cos(radLat) * pi);
        double mgLat = lat + dLat;
        double mgLon = lon + dLon;
        return new double[] { mgLat, mgLon };
    }

    /// <summary>
    /// 高德坐标 转 GPS 
    /// </summary>
    public static double[] Gcj02ToGps84(double lat, double lon)
    {
        double[] gps = transform(lat, lon);
        double lontitude = lon * 2 - gps[1];
        double latitude = lat * 2 - gps[0];
        return new double[] { latitude, lontitude };
    }

    /// <summary>
    /// 高德坐标 转 百度坐标
    /// </summary>
    public static double[] Gcj02ToBd09(double lat, double lon)
    {
        double x = lon, y = lat;
        double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
        double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
        double tempLon = z * Math.Cos(theta) + 0.0065;
        double tempLat = z * Math.Sin(theta) + 0.006;
        double[] gps = { tempLat, tempLon };
        return gps;
    }

    /// <summary>
    /// 百度坐标 转 高德坐标
    /// </summary>
    public static double[] Bd09ToGcj02(double lat, double lon)
    {
        double x = lon - 0.0065, y = lat - 0.006;
        double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
        double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
        double tempLon = z * Math.Cos(theta);
        double tempLat = z * Math.Sin(theta);
        double[] gps = { tempLat, tempLon };
        return gps;
    }

    /// <summary>
    /// GPS 转 百度坐标
    /// </summary>
    public static double[] Gps84ToBd09(double lat, double lon)
    {
        double[] gcj02 = Gps84ToGcj02(lat, lon);
        double[] bd09 = Gcj02ToBd09(gcj02[0], gcj02[1]);
        return bd09;
    }

    /// <summary>
    /// 百度坐标 转 GPS
    /// </summary>
    public static double[] Bd09ToGps84(double lat, double lon)
    {
        double[] gcj02 = Bd09ToGcj02(lat, lon);
        double[] gps84 = Gcj02ToGps84(gcj02[0], gcj02[1]);
        //保留小数点后六位  
        gps84[0] = Retain6(gps84[0]);
        gps84[1] = Retain6(gps84[1]);
        return gps84;
    }

    /// <summary>
    /// 保留小数点后六位 
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private static double Retain6(double num)
    {
        String result = String.Format("%.6f", num);
        return Double.Parse(result);
    }
}