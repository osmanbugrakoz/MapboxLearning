using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using TMPro;
using Unity.VisualScripting;
using Unity.Mathematics;
public class Mapbox : MonoBehaviour
{
    public GameObject Car;
    public RectTransform mapRectTransform;
    public Button ZoomInButton;
    public Button ZoomOutButton;
    public Button LocationButtonIstanbul;
    public Button LocationButtonAnkara;
    public TextMeshProUGUI locationtextIstanbul;
    public TextMeshProUGUI locationtextAnkara;
    public string accessToken;
    public float centerLatitude = 39.92083f;
    public float centerLongitude = 32.8541f;
    public float zoom = 12.0f;
    public int bearing = 0;
    public int pitch = 0;

    public enum style { Light, Dark, Streets, Outdoors, Satellite, SatelliteStreets };
    public style mapStyle = style.Streets;
    public enum resolution { low = 1, high = 2 };
    public resolution mapResolution = resolution.low;

    private int mapWidth = 800;
    private int mapHeight = 600;
    private string[] styleStr = new string[] { "light-v10", "dark-v10", "streets-v11", "outdoors-v11", "satellite-v9", "satellite-streets-v11" };
    private string url = "";
    private bool mapIsLoading = false;
    private Rect rect;
    private bool updateMap = true;

    private string accessTokenLast;
    private float centerLatitudeLast = 41.0082f;
    private float centerLongitudeLast = 28.9784f;
    private float zoomLast = 12.0f;
    private int bearingLast = 0;
    private int pitchLast = 0;
    private style mapStyleLast = style.Streets;
    private resolution mapResolutionLast = resolution.low;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GetMapbox());
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
        mapWidth = (int)Math.Round(rect.width);
        mapHeight = (int)Math.Round(rect.height);
        LocationButtonAnkara.onClick.AddListener(changeCitytoAnkara);
        LocationButtonIstanbul.onClick.AddListener(ChangeCitytoIstanbul);

    }

    // Update is called once per frame
    void Update()
    {
        if (updateMap && (accessTokenLast != accessToken || !Mathf.Approximately(centerLatitudeLast, centerLatitude) || !Mathf.Approximately(centerLongitudeLast, centerLongitude) || !Mathf.Approximately(zoomLast, zoom) || bearingLast != bearing
            || pitchLast != pitch || mapStyleLast != mapStyle || mapResolutionLast != mapResolution))
        {
            rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
            mapWidth = (int)Math.Round(rect.width);
            mapHeight = (int)Math.Round(rect.height);
            StartCoroutine(GetMapbox());
            updateMap = false;
        }
    }

    IEnumerator GetMapbox()
    {
        pitch = Mathf.Clamp(pitch, 0, 60);
        bearing = Mathf.Clamp(bearing, 0, 360);
        zoom = Mathf.Clamp(zoom, 0f, 22f);

        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;

        mapWidth = (int)Math.Round(rect.width);
        mapHeight = (int)Math.Round(rect.height);
        mapWidth = Mathf.Min(mapWidth, 1280);
        mapHeight = Mathf.Min(mapHeight, 1280);

        url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/" + centerLongitude + "," + centerLatitude + "," + zoom + "," + bearing + "," + pitch + "/" + mapWidth + "x" + mapHeight + "?" + "access_token=" + accessToken;
        Debug.Log("İstek URL'si: " + url);
        mapIsLoading = true;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);

        }
        else
        {
            mapIsLoading = false;
            gameObject.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            accessTokenLast = accessToken;
            centerLatitudeLast = centerLatitude;
            centerLongitudeLast = centerLongitude;
            zoomLast = zoom;
            bearingLast = bearing;
            pitchLast = pitch;
            mapStyleLast = mapStyle;
            mapResolutionLast = mapResolution;
            updateMap = true;
        }
    }
    public void ZoomIn()
    {
        if (zoom < 20f)
        {
            zoom += 1f;
            updateMap = true;
            Debug.Log("Butona tıklandı ve zoom:" + zoom);
        }
    }

    public void ZoomOut()
    {
        if (zoom > 0f)
        {
            zoom -= 1f;
            updateMap = true;
            Debug.Log("Butona tıklandı ve zoom:" + zoom);
        }
    }

    public void ChangeCitytoIstanbul()
    {
        if (Mathf.Approximately(centerLatitudeLast, 39.92083f) && Mathf.Approximately(centerLongitudeLast, 32.8541f))
        {
            centerLatitude = 41.103391f;
            centerLongitude = 28.991091f;
            updateMap = true;
            Debug.Log("İstanbul RAMS PARK");
            locationtextIstanbul.text = "RAMS PARK";
            RandomCarGeneration(centerLatitude, centerLongitude);
        }
    }
    public void changeCitytoAnkara()
    {
        if (Mathf.Approximately(centerLatitudeLast, 41.103391f) && Mathf.Approximately(centerLongitudeLast, 28.991091f))
        {
            centerLatitude = 39.92083f;
            centerLongitude = 32.8541f;
            updateMap = true;
            Debug.Log("Ankara Kızılay");
            locationtextAnkara.text = "KIZILAY";
            RandomCarGeneration(centerLatitude, centerLongitude);
        }
    }

    public void RandomCarGeneration(float targetLatitude, float targetLongitude)
    {
        float mapWidth = mapRectTransform.rect.width;
        float mapHeight = mapRectTransform.rect.height;

        float normalizedX = Mathf.InverseLerp(28.991091f, 32.8541f, targetLongitude);
        float normalizedY = Mathf.InverseLerp(41.103391f, 39.92083f, targetLatitude);

        float xPos = normalizedX * mapWidth;
        float yPos = normalizedY * mapHeight;

        Vector3 localPos = new Vector3(xPos, yPos, 0f);
        Vector3 worldPos = mapRectTransform.TransformPoint(localPos);

        Instantiate(Car, worldPos, Quaternion.identity);
    }
}
