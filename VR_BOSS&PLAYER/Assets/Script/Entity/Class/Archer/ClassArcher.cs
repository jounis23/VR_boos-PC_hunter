using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassArcher : ClassEntity
{
    [System.Serializable]
    public struct DetailStatus
    {
        public float skill1_damge;
        public float skill1_spendMp;
        
        public float skill2_damge;
        public float skill2_spendMp;

        public float skill3_damge;
        public float skill3_spendMp;

        public float skill4_spendMp;
    }
    public DetailStatus detailStatus;

    public override void Attack(Entity entity)
    {

    }


    public override void Skill1(Entity entity)
    {
        entity.status.mp -= detailStatus.skill1_spendMp;
    }

    public override void Skill2(Entity entity)
    {
        entity.status.mp -= detailStatus.skill2_spendMp;
    }

    public override void Skill3(Entity entity)
    {
        entity.status.mp -= detailStatus.skill3_spendMp;
    }

    public override void Skill4(Entity entity)
    {
        entity.status.mp -= detailStatus.skill4_spendMp;
    }
}
