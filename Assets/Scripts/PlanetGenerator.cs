using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    public void Test()
    {
        string encodedString = "{\"method\": \"retrieve_configuration\",\"params\": {\"configuration\" :\"109873839\"}}";


        JSONNode jsonNode = SimpleJSON.JSON.Parse("{\"faces\": [[1, 2, 3], [4, 5, 6]]}");
        var action = jsonNode["faces"][0][2].Value;
        Debug.Log(action.GetType());
        // var wrappedjsonArray = JsonUtility.FromJson<Polyhedron>("{\"faces\": [[1, 2, 3], [4, 5, 6]]}");

        // Debug.Log(wrappedjsonArray.faces);
    }
}