﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GraphingManager : MonoBehaviour {

		int counter = 10;
		string[] data;
		//public string val = "chw_water";

public static float position;
		private const short MAX_NUMBER_GRAPHS = 14;
		public const short MAX_NUMBER_POINTS = 20;
		GraphElement[] graphs;
		GameObject[] graphPrefab;
		public GameObject graphTemplate;
		public GameObject pointTemplate;
		public GameObject lineTemplate;

		//Time Variable Here
		private const short CHW_TOTAL = 0;
		private const short HW_TOTAL = 1;
		private const short HW_KL = 2;
		private const short CHW_KL = 3;
		private const short HW_COB = 4;
		private const short CHW_COB = 5;
		private const short HW_SE1 = 6;
		private const short CHW_SE1 = 7;
		private const short HW_SE2 = 8;
		private const short CHW_SE2 = 9;
		private const short HW_SSB = 10;
		private const short CHW_SSB = 11;
		private const short HW_SSM = 12;
		private const short CHW_SSM = 13;
		private const short HW_SAAC = 14;
		private const short CHW_SAAC = 15;
		private const short Total_Sunpower_Kwh = 16;

		Vector2 mouseLocation;

		// Use this for initialization
		void Start () {

			graphs = new GraphElement[MAX_NUMBER_GRAPHS];
			graphPrefab = new GameObject[MAX_NUMBER_GRAPHS];

			data = (Resources.Load ("data") as TextAsset).text.Split ('\n');

			for (int graph = 0; graph < MAX_NUMBER_GRAPHS; graph++) {
			//	position += MAX_NUMBER_GRAPHS/1920;
				graphs[graph] = new GraphElement (getNameOf (graph), graph);

				graphPrefab[graph] = Instantiate (graphTemplate) as GameObject;
				graphPrefab[graph].GetComponent<Transform> ().parent = GameObject.Find ("Canvas").GetComponent<Transform> ();
				graphPrefab[graph].GetComponent<RectTransform> ().position = new Vector2 (graphs[graph].positionX, graphs[graph].positionY);

				graphPrefab[graph].GetComponent<NodeManager> ().points = new GameObject[GraphingManager.MAX_NUMBER_POINTS];
				graphPrefab[graph].GetComponent<NodeManager> ().lines = new GameObject[GraphingManager.MAX_NUMBER_POINTS - 1];

				for (int point = 0; point < MAX_NUMBER_POINTS; point++) {
					GameObject pointCopy = Instantiate (pointTemplate) as GameObject;
					pointCopy.GetComponent<Transform> ().parent = graphPrefab[graph].GetComponent<Transform> ();
					graphPrefab[graph].GetComponent<NodeManager> ().points[point] = pointCopy;
				}
				for (int point = 0; point < MAX_NUMBER_POINTS - 1; point++) {
					GameObject lineCopy = Instantiate (lineTemplate) as GameObject;
					lineCopy.GetComponent<Transform> ().parent = graphPrefab[graph].GetComponent<Transform> ();
					graphPrefab[graph].GetComponent<NodeManager> ().lines[point] = lineCopy;
				}
			}
		}
		int delay = 0;
		// Update is called once per frame
		void Update () {
			counter++;
			if (true) {

				//DRAG START
				if (Input.GetMouseButtonDown (0)) {
					for (int graph = 0; graph < MAX_NUMBER_GRAPHS; graph++) {

						mouseLocation = Input.mousePosition;
						graphs[graph].over = false;

						float xMin = graphs[graph].positionX;
						float xMax = graphs[graph].positionX + graphs[graph].width;
						float yMin = graphs[graph].positionY;
						float yMax = graphs[graph].positionY + graphs[graph].height;

						if (clickOn (-graphs[graph].width / 2, graphs[graph].width / 2, -graphs[graph].height / 2, graphs[graph].height / 2, mouseLocation, new Vector2 (graphs[graph].positionX, graphs[graph].positionY))) {
							graphs[graph].over = true;
						}
					}
				}
				//DRAG MOVE
				if (Input.GetMouseButton (0)) {
					Vector2 newMouseLocation = Input.mousePosition;
					for (int graph = 0; graph < MAX_NUMBER_GRAPHS; graph++) {

						if (graphs[graph].over) {
							graphs[graph].positionX += -(mouseLocation.x - newMouseLocation.x);
							graphs[graph].positionY += -(mouseLocation.y - newMouseLocation.y);
							print (graphs[graph].positionX);
							print (graphs[graph].positionY);
						}
					}
					mouseLocation = newMouseLocation;
				}
				//FORCE UPDATE
				for (int graph = 0; graph < MAX_NUMBER_GRAPHS; graph++) {
					// if (graph == 12) continue;
					graphPrefab[graph].GetComponent<RectTransform> ().GetChild (1).GetChild (0).GetComponent<Text> ().text = "0 to " + graphs[graph].yAxisMax;
					graphPrefab[graph].GetComponent<RectTransform> ().GetChild (2).GetChild (0).GetComponent<Text> ().text = data[counter++].Split (',') [0];
					graphPrefab[graph].GetComponent<RectTransform> ().GetChild (3).GetComponent<Text> ().text = graphs[graph].graphTitle;

					//Checks if it's the sun power									//if graph == 0; index == 4
					if (graph == 8) {

						//SunPower
						//print(float.Parse(data[counter++].Split(',')[getIndexOf(graph)]));
						float val = 0;
						if (float.TryParse (data[counter].Split (',') [getIndexOf (graph)], out val)) {
							if (val > 0) {
								graphs[graph].addPoint (val);
							} else { graphs[graph].addPoint (0f); }
						} else { graphs[graph].addPoint (0f); }

					} else {
						float val = 0;
						if (float.TryParse (data[counter].Split (',') [getIndexOf (graph)], out val)) {
							if (val > 0) {
								graphs[graph].addPoint (val);
							} else { graphs[graph].addPoint (0f); }
						} else { graphs[graph].addPoint (0f); }

					}

					graphPrefab[graph].GetComponent<RectTransform> ().position = new Vector2 (graphs[graph].positionX, graphs[graph].positionY);
					graphPrefab[graph].GetComponent<RectTransform> ().sizeDelta = new Vector2 (graphs[graph].width, graphs[graph].yAxisMax*.75f + 200);
					//UPDATE NODES
					for (int point = 0; point < MAX_NUMBER_POINTS; point++) {
						Vector2 new_pos = new Vector2 ((float) point / (float) MAX_NUMBER_POINTS * (float) graphs[graph].width - graphs[graph].width / 2, graphs[graph].points[point] - graphs[graph].height / 2);
						graphPrefab[graph].GetComponent<NodeManager> ().points[point].GetComponent<RectTransform> ().localPosition = new_pos;
					}
					//UPDATE LINES
					for (int point = 0; point < MAX_NUMBER_POINTS - 1; point++) {
						//distance and theta			 
						//define x of points[point](float)point/(float)MAX_NUMBER_POINTS * (float)graphs[graph].width
						float nodePointX = (float) point / (float) MAX_NUMBER_POINTS * (float) graphs[graph].width;
						float nodePointX1 = ((float) point + 1) / (float) MAX_NUMBER_POINTS * (float) graphs[graph].width;
						float distance = Mathf.Sqrt (Mathf.Pow (nodePointX - nodePointX1, 2) + Mathf.Pow (graphs[graph].points[point] - graphs[graph].points[point + 1], 2));
						float angle = Mathf.Rad2Deg * Mathf.Atan ((graphs[graph].points[point] - graphs[graph].points[point + 1]) / (nodePointX - nodePointX1));
						float xNew = (nodePointX + nodePointX1) / 2 - graphs[graph].width / 2;
						float yNew = (graphs[graph].points[point] + graphs[graph].points[point + 1]) / 2 - graphs[graph].height / 2; // * th

						graphPrefab[graph].GetComponent<NodeManager> ().lines[point].GetComponent<RectTransform> ().sizeDelta = new Vector2 (distance, 5);
						graphPrefab[graph].GetComponent<NodeManager> ().lines[point].GetComponent<RectTransform> ().localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
						graphPrefab[graph].GetComponent<NodeManager> ().lines[point].GetComponent<RectTransform> ().localPosition = new Vector2 (xNew, yNew);

					}
				}
			}

		}

		//CHECKS IF CLICK IS WITHIN GIVEN BOUNDS
		public bool clickOn (float xMin, float xMax, float yMin, float yMax, Vector2 mousePosition, Vector2 otherPosition) {
			if (mousePosition.x + xMin < otherPosition.x && mousePosition.x + xMax > otherPosition.x)
				if (mousePosition.y + yMin < otherPosition.y && mousePosition.y + yMax > otherPosition.y) return true;
			return false;
		}
		// //CHECKS IF CLICK IS WITHIN GIVEN BOUNDS
		// public bool clickOn(float xMin, float xMax, float yMin, float yMax, Vector2 mousePosition)
		// {
		//     if (mousePosition.x > xMin  && mousePosition.x <  xMax)
		// 	 if (mousePosition.y > yMin && mousePosition.y < yMax) 
		// 	 	return true;
		//     return false;
		// }

		public string getNameOf (int value) {
			switch (value) {
				case 0:
					return "Hot Water Total";
				case 1:
					return "Chilled Water Total";
				case 2:
					return "Hot Water KOL";
				case 3:
					return "Chilled Water KOL";
				case 4:
					return "Hot Water COB";
				case 5:
					return "Chilled Water COB";
				case 6:
					return "Hot Water SE1";
				case 7:
					return "Chilled Water SE1";
				case 8:
					return "Hot Water SE2";
				case 9:
					return "Chilled Water SE2";
				case 10:
					return "Hot Water SSB";
				case 11:
					return "Chilled Water SSB";
				case 12:
					return "Hot Water SSM";
				case 13:
					return "Chilled Water SSM";
				case 14:
					return "Hot Water SAAC";
				case 15:
					return "Chilled Water SAAC";
				case 16:
					return "Total SunPower";
			}
			return "";
		}

		public int getIndexOf (int value) {
			return value;
		}

		public bool hotCold (int value) {
			switch (value) {
				case 1:
					return true;
				case 2:
					return false;
				case 3:
					return true;
				case 4:
					return true;
				case 5:
					return false;
				case 6:
					return true;
				case 7:
					return false;
				case 8:
					return true;
				case 9:
					return false;
				case 10:
					return true;
				case 11:
					return false;
				case 12:
					return true;
				case 13:
					return false;
				case 14:
					return true;
				case 15:
					return false;
			}
			return false;
		}
}
