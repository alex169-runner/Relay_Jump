using System.Collections.Generic;
using UnityEngine;

internal class FrogManager : MonoBehaviour
{
    public static int number = 4;
    public int stimulatedId;

    public static readonly int[] initX = { 9, 10, 9, 10 };
    public static readonly int[] initY = { 9, 9, 10, 10 };

    public Map map;
    public List<Frog> frogs;
    public GameObject frogPrefab;

    public void Clear()
    {
        map = null;
        for (int i = 0; i < frogs.Count; i++) {
            Destroy(frogs[i].gameObject);
        }
        frogs.Clear();
    }

    public void Init()
    {
        Clear();
        map = new();
        for (int i = 0; i < number; ++i) {
            GameObject frogObj = Instantiate(frogPrefab);
            Frog frog = frogObj.GetComponent<Frog>();
            frog.id = i;
            frog.pos = new(initX[i], initY[i]);
            map.GetOccupied(frog.pos);
            frogs.Add(frog);
        }

        stimulatedId = Random.Range(0, number);
        frogs[stimulatedId].isStimulated = true;
    }

    private void Awake()
    {
        frogs = new List<Frog>();
    }

    public void Jump(Frog centre)
    {
        if (centre.isStimulated) {
            return;
        }

        Vector2Int reflectPos = Map.Reflection(frogs[stimulatedId].pos, centre.pos);

        if (map.IsOccupied(reflectPos)) {
            return;
        }

        map.GetOccupied(reflectPos);
        map.GetVacant(frogs[stimulatedId].pos);

        centre.isStimulated = true;
        frogs[stimulatedId].isStimulated = false;

        GameManager.Instance.AddStack(centre.id, stimulatedId, frogs[stimulatedId].pos, reflectPos);

        frogs[stimulatedId].pos = reflectPos;
        frogs[stimulatedId].isHome = GameManager.Instance.answer.IsOccupied(reflectPos);


        stimulatedId = centre.id;
    }

}
