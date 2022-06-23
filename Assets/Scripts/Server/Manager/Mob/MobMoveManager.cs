using System;
using System.Collections.Generic;

using UnityEngine;

public class MobMoveManager : MonoBehaviour
{
    public int Index;

    private List<GameObject> casesChemin;

    public event Action OnDeath;

    public GameObject Map { get; set; }

    public GameObject MobInScene { get; set; }

    public float Speed { get; set; }

    public void Initialize(GameObject map, GameObject mobInScene, float speed)
    {
        this.Map = map;
        this.MobInScene = mobInScene;
        this.Speed = speed;
    }

    private void Deplacement()
    {
        this.MobInScene.transform.position = Vector3.MoveTowards(this.MobInScene.transform.position, this.casesChemin[this.Index].transform.position, this.Speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (this.Map != null)
        {
            if (this.casesChemin != null && this.casesChemin.Count == 0)
            {
                for (int i = 0; i < this.Map.transform.childCount; i++)
                {
                    this.casesChemin.Add(this.Map.transform.GetChild(i).gameObject);
                }

                this.Path();
            }
            else
            {
                this.Path();
            }
        }
    }

    private void Path()
    {
        if (this.Index >= this.casesChemin.Count)
        {
            this.OnDeath?.Invoke();
        }
        else
        {
            if (Math.Round(this.MobInScene.transform.position.x - this.casesChemin[this.Index].transform.position.x, 2) == 0 && Math.Round(this.MobInScene.transform.position.y - this.casesChemin[this.Index].transform.position.y, 2) == 0)
            {
                this.Index++;
                return;
            }

            this.Deplacement();
        }
    }

    private void Start()
    {
        this.Index = 0;
        this.casesChemin = new List<GameObject>();
    }
}