using UnityEngine;
using System.Collections;
using System;

public class SceneGeneration : MonoBehaviour
{

    /*
     * codu sursa
     * pe laptop cum functioneaza
     * o prezentare in care explicam cum ne-am organizat, lessons learned and feedback
     * documentatie de utilizare a proiectului, nu foarte mare
     * documentatie tehnica
     */

    public Transform brick;
    public static int width = 19;
    public static int height = 27;
    public static int limit_w = (width - 1) / 2;
    public static int limit_h = (height - 1) / 2;
    bool[,] grid = new bool[height, width];
    public static System.Random rnd = new System.Random(1);

    public void generateStructure(int x, int y)
    {
        int[] xs = new int[4];
        int[] ys = new int[4];

        xs[0] = x - 1; ys[0] = y;
        xs[1] = x; ys[1] = y - 1;
        xs[2] = x + 1; ys[2] = y;
        xs[3] = x; ys[3] = y + 1;

        int rnd_no = rnd.Next(0, 4);
        int chosen_x = xs[rnd_no];
        int chosen_y = ys[rnd_no];

        for (int i = (x - 1); i <= (x + 1); i++)
            for (int j = (y - 1); j <= (y + 1); j++)
            {
                if ((i == chosen_x) && (j == chosen_y))
                    continue;

                if (i == x && j == y)
                    continue;
                Instantiate(brick, new Vector3(i, j, 1), Quaternion.identity);
                grid[i + limit_h, j + limit_w] = true;
            }


    }



    // Use this for initialization
    void Start()
    {

        // Generate outer grid
        for (int i = -limit_h; i <= limit_h; i++)
        {
            grid[(i + limit_h), 0] = true;
            Instantiate(brick, new Vector3(i, -limit_w, 1), Quaternion.identity);
            grid[(i + limit_h), (width - 1)] = true;
            Instantiate(brick, new Vector3(i, limit_w, 1), Quaternion.identity);
        }

        for (int j = -limit_w; j <= limit_w; j++)
        {
            grid[0, (j + limit_w)] = true;
            Instantiate(brick, new Vector3(-limit_h, j, 1), Quaternion.identity);
            grid[(height - 1), (j + limit_w)] = true;
            Instantiate(brick, new Vector3(limit_h, j, 1), Quaternion.identity);
        }

        for (int i = (-limit_h + 3); i <= (limit_h - 3); i += 4)
            for (int j = (-limit_w + 3); j <= (limit_w - 3); j += 4)
                generateStructure(i, j);
    }

    /*
	// Update is called once per frame
	void Update () {
	
	}
    */
}
