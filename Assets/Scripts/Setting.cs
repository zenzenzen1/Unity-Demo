using UnityEngine;

public static class Setting {
    public static string defaultDisplayName = "30_NguyenXuanTruong_Lab1";
    public static int asteroidScore = 10;
    public static int starScore = 5;
    
    public static Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    public static float minX = -bounds.x;
    public static float maxX = bounds.x;
    public static float minY = -bounds.y;
    public static float maxY = bounds.y;
    public static float centerX = (minX + maxX) / 2;
    public static float centerY = (minY + maxY) / 2;
}