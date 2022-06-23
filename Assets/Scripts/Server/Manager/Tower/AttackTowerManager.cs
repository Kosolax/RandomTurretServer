using System;

using UnityEngine;

public class AttackTowerManager : MonoBehaviour
{
    private DateTime oldDate;

    public event Action CanShoot;

    public float AttackSpeed { get; set; }

    public void Initialize(float attackSpeed)
    {
        this.AttackSpeed = attackSpeed;
    }

    private void Start()
    {
        this.oldDate = DateTime.Now;
    }

    private void Update()
    {
        if ((DateTime.Now - this.oldDate).TotalSeconds > 1 / this.AttackSpeed)
        {
            this.oldDate = DateTime.Now;
            this.CanShoot?.Invoke();
        }
    }
}