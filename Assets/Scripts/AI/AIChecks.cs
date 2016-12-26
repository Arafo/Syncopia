using UnityEngine;
using System.Collections.Generic;

public class AIChecks {
    private ShipReferer ship;

    private bool isSetup;
    private int i;
    private byte j;

    private Vector3 thisPosition;
    private Vector3 targetPosition;

    public AIChecks(ShipReferer ship) {
        this.ship = ship;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentData"></param>
    /// <returns></returns>
    public AIData[] GetAwarenessData(AIData[] currentData) {
        currentData = AwarenessDataIntegrityCheck(currentData);
        if (isSetup) {
            currentData = AwarenessOffsetCheck(currentData);
            currentData = AwarenessSituationEvaluation(currentData);
        }
        else {
            isSetup = true;
        }

        return currentData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="awarenessData"></param>
    /// <returns></returns>
    private AIData[] AwarenessDataIntegrityCheck(AIData[] awarenessData) {
        if (awarenessData.Length != RaceSettings.ships.Count - 1) {
            List<AIData> list = new List<AIData>();
            i = 0;
            while (i < RaceSettings.ships.Count) {
                if (!(RaceSettings.ships[i] == ship)) {
                    list.Add(new AIData(RaceSettings.ships[i]));
                }
                i++;
            }
            awarenessData = list.ToArray();
        }
        return awarenessData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="awarenessData"></param>
    /// <returns></returns>
    private AIData[] AwarenessOffsetCheck(AIData[] awarenessData) {
        i = 0;
        while (i < awarenessData.Length) {
            awarenessData[i].localOffset = awarenessData[i].ship.transform.InverseTransformPoint(ship.transform.position);
            awarenessData[i].segmentOffset = Mathf.RoundToInt(Mathf.Repeat((float)(awarenessData[i].ship.currentSection.index - ship.currentSection.index), (float)RaceSettings.raceManager.trackData.trackData.sections.Count));
            i++;
        }

        return awarenessData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="awarenessData"></param>
    /// <returns></returns>
    private AIData[] AwarenessSituationEvaluation(AIData[] awarenessData) {
        j = 0;
        while ((int)j < awarenessData.Length) {
            awarenessData[(int)j].shipInFront = (awarenessData[(int)j].localOffset.z < 0f && awarenessData[(int)j].localOffset.z > -10f);
            awarenessData[(int)j].shipBehind = (awarenessData[(int)j].localOffset.z > 0f && awarenessData[(int)j].localOffset.z < 10f);
            awarenessData[(int)j].shipRight = (awarenessData[(int)j].localOffset.x < 0f && awarenessData[(int)j].localOffset.x > -1.5f && Mathf.Abs(awarenessData[(int)j].localOffset.z) < 6f);
            awarenessData[(int)j].shipLeft = (awarenessData[(int)j].localOffset.x > 0f && awarenessData[(int)j].localOffset.x < 1.5f && Mathf.Abs(awarenessData[(int)j].localOffset.z) < 6f);
            j += 1;
        }

        return awarenessData;
    }
}
