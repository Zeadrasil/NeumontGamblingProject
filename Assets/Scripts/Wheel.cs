using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    //Each tile in the wheel, first 2 and last 2 should be the same type in order to allow all to be seen
    [SerializeField] private List<Tile> tiles;
    //Size of each tile's sprite in world units
    [SerializeField] float size;
    //Speed in tiles per second
    [SerializeField] private float speed;
    //Current tile that the wheel is showing
    [SerializeField] private float current = 1;

    [SerializeField] private GameObject[] tilePrefabs;

    public bool spinning = true;
    public bool fixing = false;
    // Update is called once per frame
    void Update()
    {
        if (spinning)
        {
            current += speed * Time.deltaTime;
            if (current >= tiles.Count - 1.5f)
            {
                current -= (int)current;
            }
            Move();
        }
        else if (fixing)
        {
            int goal = Mathf.RoundToInt(current);
            if (current < goal)
            {
                current += speed * Time.deltaTime * 0.1f;
                if (current >= goal)
                {
                    current = goal;
                    fixing = false;
                }
            }
            else if (current > goal)
            {
                current -= speed * Time.deltaTime * 0.1f;
                if (current <= goal)
                {
                    current = goal;
                    fixing = false;
                }
            }
            else
            {
                fixing = false;
            }
            Move();
        }
    }

    private void Move()
    {
        for (int index = 0; index < tiles.Count; index++)
        {
            tiles[index].gameObject.transform.position = new Vector3(transform.position.x, (index - current) * size + transform.position.y);
        }
    }

    private async void Start()
    {
        List<AsyncInstantiateOperation<GameObject>> creationHolder = new List<AsyncInstantiateOperation<GameObject>>();
        for(int index = 0; index < tilePrefabs.Length; index++)
        {
            creationHolder.Add(InstantiateAsync(tilePrefabs[index], tilePrefabs.Length - index));
        }
        List<GameObject> objectHolder = new List<GameObject>();
        while(creationHolder.Count > 0)
        {
            if (creationHolder[0].isDone)
            {
                foreach(GameObject go in creationHolder[0].Result)
                {
                    if(objectHolder.Count != 0)
                    {
                        objectHolder.Insert(Random.Range(0, objectHolder.Count + 1), go);
                    }
                    else
                    {
                        objectHolder.Add(go);
                    }
                }
                creationHolder.RemoveAt(0);
            }
            else
            {
                await creationHolder[0];
            }
        }
        foreach(GameObject go in objectHolder)
        {
            tiles.Add(go.GetComponent<Tile>());
        }
        tiles.Add(Instantiate(tiles[0]));
        tiles.Add(Instantiate(tiles[1]));
    }

    public int Result {  get { return tiles[Mathf.RoundToInt(current)].type; } }
}
