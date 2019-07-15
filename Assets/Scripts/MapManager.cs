using UnityEngine;
using System.Collections;
using System.Xml;
using System;

public class MapManager {

    private Hex hex; //Hex prefab

    private string mapName;
    private int heightMap; //length
    private int widthMap;

    private float hexHeight;
    private float hexWidth;

    private Hex[][] hexMap;

    private class HexPathData
    {
        public Hex[] mainPath;
        public Hex[] reservePath;
        public int distance;
    }

    private HexPathData[][] hexPathData;

    private class PathData
    {
        public Hex[] path = null;
        public int distance = -1;
    }

    private PathData currentPath = new PathData();

    public MapManager()
    {

    }

    public void Show()
    {
        Debug.Log(currentPath.distance);
        foreach (Hex h in currentPath.path)
        {
            Debug.Log(h.X + " " + h.Y);
        }
    }
    
    public MapManager (string map)
    {
        string obstacles;

        obstacles = ReadMapDataXML(map);

        CalculateSizeHex();
        GenerateHexMap();
        LinkHexMap();
        AddHexMapObstacles(obstacles);

        GenerateHexPathData();
    }

    private string ReadMapDataXML (string map)
    {
        XmlDocument maps = new XmlDocument();
        maps.Load("Assets/Maps/mapdata.xml");
        XmlNode root = maps.SelectSingleNode("/maps/" + map);
        this.mapName = root["mapname"].InnerText;
        this.heightMap = Int32.Parse(root["height"].InnerText);
        this.widthMap = Int32.Parse(root["width"].InnerText);

        XmlNode obstacles_root = maps.SelectSingleNode("//obstacles");

        return obstacles_root.OuterXml;
    }

    private void CalculateSizeHex ()
    {
        GameObject tempHex = Resources.Load("Hex") as GameObject;

        Mesh hexMesh = tempHex.GetComponent<MeshFilter>().sharedMesh;

        hexWidth = hexMesh.bounds.size.x;
        hexHeight = hexMesh.bounds.size.z;
    }

    private void GenerateHexMap ()
    {
        hexMap = new Hex[widthMap][];
        for (int i = 0; i < hexMap.Length; i++)
        {
            if (i % 2 != 0)
            {
                hexMap[i] = new Hex[heightMap - 1];
            }
            else
            {
                hexMap[i] = new Hex[heightMap];
            }
        }

        GameObject hex = Resources.Load("Hex") as GameObject;
        GameObject hexMapGO = GameObject.Find("HexMap");

        for (int i = 0; i < hexMap.Length; i++)
        {
            for (int j = 0; j < hexMap[i].Length; j++)
            {
                GameObject newHex = MonoBehaviour.Instantiate(hex);

                if (i % 2 != 0)
                {
                    newHex.transform.position = new Vector3((hexWidth* 3 / 4) * i, 0.0f, hexHeight * j + hexHeight / 2);
                }
                else
                {
                    newHex.transform.position = new Vector3((hexWidth * 3 / 4) * i, 0.0f, hexHeight * j);
                }

                newHex.name = "Hex[" + i + "," + j + "]";
                newHex.transform.parent = hexMapGO.transform;

                newHex.GetComponent<Hex>().X = i;
                newHex.GetComponent<Hex>().Y = j;
                newHex.GetComponent<Hex>().isMove = true;
                newHex.GetComponent<Hex>().isShoot = true;

                hexMap[i][j] = newHex.GetComponent<Hex>();
            }
        }
    }

    private void LinkHexMap ()
    {
        //Link neighbors L 1
        for (int i = 0; i < hexMap.Length; i++)
        {
            for (int j = 0; j < hexMap[i].Length; j++)
            {
                Hex hex = hexMap[i][j];

                for (int index = 0; index < 6; index++)
                {

                    if (i % 2 != 0)
                    {
                        switch (index)
                        {
                            case 0:
                                if (i > 0)
                                    hex.neighbors[index] = hexMap[i - 1][j];
                                break;
                            case 1:
                                if (i > 0)
                                    hex.neighbors[index] = hexMap[i - 1][j + 1];
                                break;
                            case 2:
                                if (j < hexMap[i].Length - 1)
                                    hex.neighbors[index] = hexMap[i][j + 1];
                                break;
                            case 3:
                                if (i < hexMap.Length - 1)
                                    hex.neighbors[index] = hexMap[i + 1][j + 1];
                                break;
                            case 4:
                                if (i < hexMap.Length - 1)
                                    hex.neighbors[index] = hexMap[i + 1][j];
                                break;
                            case 5:
                                if (j > 0)
                                    hex.neighbors[index] = hexMap[i][j - 1];
                                break;
                        }
                    }
                    else
                    {
                        switch (index)
                        {
                            case 0:
                                if (i > 0 && j > 0)
                                    hex.neighbors[index] = hexMap[i - 1][j - 1];
                                break;
                            case 1:
                                if (i > 0 && j < hexMap[i].Length - 1)
                                    hex.neighbors[index] = hexMap[i - 1][j];
                                break;
                            case 2:
                                if (j < hexMap[i].Length - 1)
                                    hex.neighbors[index] = hexMap[i][j + 1];
                                break;
                            case 3:
                                if (i < hexMap.Length - 1 && j < hexMap[i].Length - 1)
                                    hex.neighbors[index] = hexMap[i + 1][j];
                                break;
                            case 4:
                                if (i < hexMap.Length - 1 && j > 0)
                                    hex.neighbors[index] = hexMap[i + 1][j - 1];
                                break;
                            case 5:
                                if (j > 0)
                                    hex.neighbors[index] = hexMap[i][j - 1];
                                break;
                        }
                    }
                }
            }
        }

        //Link neighbors L 2
        /*for (int i = 0; i < hexMap.Length; i++)
        {
            for (int j = 0; j < hexMap[i].Length; j++)
            {
                Hex hex = hexMap[i][j];

                for (int index = 0; index < 6; index++)
                {
                    if (i % 2 != 0)
                    {
                        switch (index)
                        {
                            case 0:
                                if (i > 0 && j > 0)
                                    hex.neighborsL2[index] = hexMap[i - 1][j - 1];
                                break;
                            case 1:
                                if (i > 1)
                                    hex.neighborsL2[index] = hexMap[i - 2][j];
                                break;
                            case 2:
                                if (i > 0 && j < hexMap[i].Length - 2)
                                    hex.neighborsL2[index] = hexMap[i - 1][j + 2];
                                break;
                            case 3:
                                if (i < hexMap.Length - 1 && j < hexMap[i].Length - 2)
                                    hex.neighborsL2[index] = hexMap[i + 1][j + 2];
                                break;
                            case 4:
                                if (i < hexMap.Length - 2)
                                    hex.neighborsL2[index] = hexMap[i + 2][j];
                                break;
                            case 5:
                                if (i < hexMap.Length - 1 && j > 0)
                                    hex.neighborsL2[index] = hexMap[i + 1][j - 1];
                                break;
                        }
                    }
                    else
                    {
                        switch (index)
                        {
                            case 0:
                                if (i > 0 && j > 1)
                                    hex.neighborsL2[index] = hexMap[i - 1][j - 2];
                                break;
                            case 1:
                                if (i > 1)
                                    hex.neighborsL2[index] = hexMap[i - 2][j];
                                break;
                            case 2:
                                if (i > 0 && j < hexMap[i].Length - 2)
                                    hex.neighborsL2[index] = hexMap[i - 1][j + 1];
                                break;
                            case 3:
                                if (i < hexMap.Length - 1 && j < hexMap[i].Length - 2)
                                    hex.neighborsL2[index] = hexMap[i + 1][j + 1];
                                break;
                            case 4:
                                if (i < hexMap.Length - 2)
                                    hex.neighborsL2[index] = hexMap[i + 2][j];
                                break;
                            case 5:
                                if (i < hexMap.Length - 1 && j > 1)
                                    hex.neighborsL2[index] = hexMap[i + 1][j - 2];
                                break;
                        }
                    }
                }
            }
        }*/
    }

    private void AddHexMapObstacles (string obstacles)
    {
        XmlDocument xml_string = new XmlDocument();
        xml_string.LoadXml(obstacles);

        XmlNode root_obstacles = xml_string.SelectSingleNode("/obstacles");

        foreach (XmlNode obstacle in root_obstacles)
        {
            int x = Int32.Parse(obstacle.Attributes["x"].Value);
            int y = Int32.Parse(obstacle.Attributes["y"].Value);

            string elemData = obstacle.InnerText;
            string[] splitElemData = elemData.Split(',');

            hexMap[x][y].isMove = splitElemData[0] == "1";
            hexMap[x][y].isShoot = splitElemData[1] == "1";
        }
    }

    private void GenerateHexPathData()
    {
        hexPathData = new HexPathData[widthMap][];
        for (int i = 0; i < hexPathData.Length; i++)
        {
            if (i % 2 != 0)
            {
                hexPathData[i] = new HexPathData[heightMap - 1];
            }
            else
            {
                hexPathData[i] = new HexPathData[heightMap];
            }
        }

        for (int i = 0; i < hexPathData.Length; i++)
        {
            for (int j = 0; j < hexPathData[i].Length; j++)
            {
                hexPathData[i][j] = new HexPathData();
            }
        }
    }
    
    public void StartNewRound ()
    {
        ResetHexPathData();
    }

    private void ResetHexPathData()
    {
        for (int i = 0; i < hexPathData.Length; i++)
        {
            for (int j = 0; j < hexPathData[i].Length; j++)
            {
                if (hexPathData[i][j].distance != -1)
                    hexMap[i][j].SetDefaultColor();
            }
        }

        for (int i = 0; i < hexPathData.Length; i++)
        {
            for (int j = 0; j < hexPathData[i].Length; j++)
            {
                hexPathData[i][j].mainPath = null;
                hexPathData[i][j].reservePath = null;
                hexPathData[i][j].distance = -1;
            }
        }
    }

    public void FindAvailablePathForward (int x, int y, int indexStartDirection, int pointsMove)
    {
        if (pointsMove == 0) return;

        HexPathData hexData = hexPathData[x][y];
        hexData.mainPath = new Hex[] { hexMap[x][y] };
        hexData.distance = 0;

        Hex[] neighbors = hexMap[x][y].neighbors;
        //Hex[] neighborsL2 = hexMap[x][y].neighborsL2;

        for (int i = 0; i < 6; i++)
        {
            if (neighbors[i] != null)
                CalculateAvailablePathForward(neighbors[i].X, neighbors[i].Y, hexData.mainPath, hexData.distance, indexStartDirection, i, pointsMove);
        }
    }

    private void CalculateAvailablePathForward(int x, int y, Hex[] way, int currentDistance, int indexStartDirection, int indexEndDirection, int pointsMove)
    {
        if (hexMap[x][y].isMove == false) return;
        //calculate distance to move there
        //calculate forward rotates
        int rotates = System.Math.Abs(indexEndDirection - indexStartDirection);
        if (rotates > 3) rotates = 6 - rotates;
        //calculate forward move
        int distance = GameLogicSettings.tankForwardMovePoint + rotates * GameLogicSettings.tankRotateMovePoint;
        //calculate start back index
        if (currentDistance + distance > pointsMove) return;

        HexPathData cellData = hexPathData[x][y];

        if (cellData.distance != -1 && cellData.distance < currentDistance + distance) return;
        Hex[] newWay = new Hex[way.Length + 1];
        System.Array.Copy(way, newWay, way.Length);
        newWay[newWay.Length - 1] = hexMap[x][y];
        if (cellData.distance == currentDistance + distance)
        {
            cellData.reservePath = newWay;
        }
        else
        {
            cellData.mainPath = newWay;
        }
        cellData.distance = currentDistance + distance;

        Hex[] neighbors = hexMap[x][y].neighbors;

        for (int i = 0; i < 6; i++)
        {
            if (neighbors[i] != null)
                CalculateAvailablePathForward(neighbors[i].X, neighbors[i].Y, cellData.mainPath, cellData.distance, indexEndDirection, i, pointsMove);
        }
    }

    public void FindAvailablePathBackward (int x, int y, int indexStartDirection, int pointsMove)
    {
        if (pointsMove == 0) return;

        HexPathData hexData = hexPathData[x][y];
        hexData.mainPath = new Hex[] { hexMap[x][y] };
        hexData.distance = 0;

        Hex[] neighbors = hexMap[x][y].neighbors;
        //Hex[] neighborsL2 = hexMap[x][y].neighborsL2;

        for (int i = 0; i < 6; i++)
        {

            if (neighbors[i] != null)
                CalculateAvailablePathBackward(neighbors[i].X, neighbors[i].Y, hexData.mainPath, hexData.distance, indexStartDirection, i, pointsMove);
        }
    }

    private void CalculateAvailablePathBackward(int x, int y, Hex[] way, int currentDistance, int indexStartDirection, int indexEndDirection, int pointsMove)
    {
        if (hexMap[x][y].isMove == false) return;
        //calculate distance to move there
        //calculate forward rotates
        int rotates = System.Math.Abs(indexEndDirection - indexStartDirection);
        if (rotates > 3) rotates = 6 - rotates;
        //calculate forward move
        int distance = GameLogicSettings.tankBackwardMovePoint + rotates * GameLogicSettings.tankRotateMovePoint;
        //calculate start back index
        if (currentDistance + distance > pointsMove) return;

        HexPathData cellData = hexPathData[x][y];

        if (cellData.distance != -1 && cellData.distance < currentDistance + distance) return;
        Hex[] newWay = new Hex[way.Length + 1];
        System.Array.Copy(way, newWay, way.Length);
        newWay[newWay.Length - 1] = hexMap[x][y];
        if (cellData.distance == currentDistance + distance)
        {
            cellData.reservePath = newWay;
        }
        else
        {
            cellData.mainPath = newWay;
        }
        cellData.distance = currentDistance + distance;

        Hex[] neighbors = hexMap[x][y].neighbors;

        for (int i = 0; i < 6; i++)
        {
            if (neighbors[i] != null)
                CalculateAvailablePathBackward(neighbors[i].X, neighbors[i].Y, cellData.mainPath, cellData.distance, indexEndDirection, i, pointsMove);
        }
    }

    public void ColorizeAvailablePath()
    {
        for (int i = 0; i < hexPathData.Length; i++)
        {
            for (int j = 0; j < hexPathData[i].Length; j++)
            {
                if (hexPathData[i][j].distance != -1)
                    hexMap[i][j].SetAvailablePathColor();
            }
        }
    }

    private void ResetMovePath ()
    {
        if (currentPath.distance == -1) return;

        foreach (Hex hex in currentPath.path)
        {
            hex.SetAvailablePathColor();
        }

        currentPath = new PathData();
    }

    public void FindMovePathForward (int x, int y, int indexEndDirection, int pointsMove)
    {
        ResetMovePath();

        if (hexPathData[x][y].distance == -1) return;

        if (hexPathData[x][y].distance == 0) return;

        if (hexPathData[x][y].reservePath != null)
        {
            Hex[] mainPath = hexPathData[x][y].mainPath;
            Hex[] reservePath = hexPathData[x][y].reservePath;

            int mainPathDistance = CalculateMovePathDistance(x, y, mainPath, indexEndDirection);
            int reservePathDistance = CalculateMovePathDistance(x, y, reservePath, indexEndDirection);

            if (mainPathDistance >= reservePathDistance)
            {
                currentPath.path = mainPath;
                currentPath.distance = mainPathDistance;
            }
            else
            {
                currentPath.path = reservePath;
                currentPath.distance = reservePathDistance;
            }
        }
        else
        {
            currentPath.path = hexPathData[x][y].mainPath;
            currentPath.distance = CalculateMovePathDistance(x, y, hexPathData[x][y].mainPath, indexEndDirection);
        }
    }

    private int CalculateMovePathDistance(int x, int y, Hex[] path, int indexEndDirection)
    {
        Hex penultimateHex = path[path.Length - 2];
        Hex[] targetHexNeighbors = hexMap[x][y].neighbors;

        int neighborIndex = 0;
        for (int i = 0; i < 6; i++)
        {
            if (targetHexNeighbors[i] == penultimateHex)
            {
                neighborIndex = i;
                break;
            }
        }

        int indexStartDirection = neighborIndex + 3;
        if (indexStartDirection > 5) indexStartDirection -= 6;

        int rotates = System.Math.Abs(indexEndDirection - indexStartDirection);
        if (rotates > 3) rotates = 6 - rotates;

        int distance = rotates * GameLogicSettings.tankRotateMovePoint + hexPathData[x][y].distance;

        return distance;
    }

    public void ColorizeMovePath ()
    {
        foreach (Hex hex in currentPath.path)
        {
            hex.SetMovePathColor();
        }
    }
}