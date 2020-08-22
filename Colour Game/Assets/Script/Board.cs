using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UnityEditor.UIElements;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int height;
    public int width;
    public GameObject[] dots;
    public GameObject[] dotsMixture;
    public GameObject[] connectors;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    public List<GameObject> mixedDot;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        mixedDot = new List<GameObject>();

        allDots = new GameObject[width, height];
        Setup();
    }

    public void Setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                if (tempPosition.x != width - 1 || tempPosition.y != height - 1)
                {
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + ", " + j + ")";

                    int dotToUse = UnityEngine.Random.Range(0, dots.Length);
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + ", " + j + ")";
                    allDots[i, j] = dot;
                }

            }
        }
        Vector2 dotsMixturePosition = new Vector2(width - 1, height - 1);
        //int dotMixedToUse = UnityEngine.Random.Range(0, dotsMixture.Length);
        GameObject dotMixed = Instantiate(dotsMixture[2], dotsMixturePosition, Quaternion.identity);
        allDots[width - 1, height - 1] = dotMixed;

        mixedDot.Add(allDots[height - 1, width - 1]);
        mixedDot.Add(allDots[height - 1, width - 1]);
        Debug.Log(mixedDot.Count);
    }

    private void DestroySwipesAt(int column, int row)
    {

        if (allDots[column, row].GetComponent<Dot>().isMoved)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }



    public bool DestroySwipes()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroySwipesAt(i, j);
                }
            }
        }
        return true;
    }

    private GameObject DotNotDestroyed()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null && allDots[i, j].GetComponent<Dot>().thisDotMoved == true)
                {
                    return allDots[i, j];
                }
            }
        }
        return null;
    }

    public void RefillSwipes()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (DestroySwipes() == true)
                {
                    if (allDots[i, j] == null)
                    {
                        Vector2 tempPosition = new Vector2(i, j);

                        if (DotNotDestroyed().tag == "Red Dot")
                        {
                            GameObject piece = Instantiate(dots[1], tempPosition, Quaternion.identity);
                            allDots[i, j] = piece;
                        }

                        else if (DotNotDestroyed().tag == "Blue Dot")
                        {
                            GameObject piece = Instantiate(dots[0], tempPosition, Quaternion.identity);
                            allDots[i, j] = piece;
                        }

                        else if (DotNotDestroyed().tag == "Yellow Dot")
                        {
                            GameObject piece = Instantiate(dots[2], tempPosition, Quaternion.identity);
                            allDots[i, j] = piece;
                        }
                    }
                }
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {

        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public bool DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        return true;
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (DestroyMatches() == true)
                {
                    if (allDots[i, j] == null)
                    {
                        Vector2 tempPosition = new Vector2(i, j);
                    
                        if (allDots[width - 1, height - 1].GetComponent<Dot>().tag == "Orange Dot")
                        {
                            GameObject piece = Instantiate(dotsMixture[2], tempPosition, Quaternion.identity);
                            allDots[i, j] = piece;
                            mixedDot.Add(allDots[i, j]);
                            Debug.Log(mixedDot.Count);
                        }
                        if (allDots[width - 1, height - 1].GetComponent<Dot>().tag == "Green Dot")
                        {
                            GameObject piece = Instantiate(dotsMixture[0], tempPosition, Quaternion.identity);
                            allDots[i, j] = piece;
                            /*Vector2 tempPosition1 = new Vector2((2 * piece.transform.position.x + 1) / 2, piece.transform.position.y);
                            if (allDots[i, j].tag == "Green Dot" && allDots[i+1, j].tag == "Green Dot")
                            {
                                GameObject connector = Instantiate(connectors[2], tempPosition1, Quaternion.Euler(0, 0, 90));
                            }*/
                        }
                        if (allDots[width - 1, height - 1].GetComponent<Dot>().tag == "Magenta Dot")
                        {
                            GameObject piece = Instantiate(dotsMixture[1], tempPosition, Quaternion.identity);
                            allDots[i, j] = piece;
                            /*Vector2 tempPosition1 = new Vector2((2 * piece.transform.position.x + 1) / 2, piece.transform.position.y);
                            if (allDots[i, j].tag == "Magenta Dot")
                            {
                                GameObject connector = Instantiate(connectors[1], tempPosition1, Quaternion.Euler(0, 0, 90));
                            }*/
                        }
                    }
                }
            }
        }
    }



    public IEnumerator FillBoardCo()
    {

        RefillBoard();
        yield return new WaitForSeconds(.5f);
    }
}