using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// custom scripts
// using static PyzymeWordSearch;

public class PyzymeGameplay
{
    private static Transform start_obj = null;
// should not swap when that doesnt make a word
// for testing we will just swap

    public static void swap_tiles(string state, Transform obj_transform, System.Action check_colwords){
    // unpack tuple
        if (state.Equals("Began")){
        // Debug.Log(obj_transform.gameObject.name);
        start_obj = obj_transform;
        }
        else if (state.Equals("Ended")){
            
        // Debug.Log(obj_transform.gameObject.name);
            if (start_obj && obj_transform ){
                Vector2 temp_pos = start_obj.localPosition;
                if (!is_swappable(temp_pos, obj_transform.localPosition)){
                    return;
                }
                Debug.Log("Swapped");
                Debug.Log(temp_pos);
                Vector2 list_ind = get_arr_ind_from_position(temp_pos);
                Debug.Log(list_ind);
                GridMaker.tiles[(int)list_ind.x][(int)list_ind.y] = obj_transform.gameObject;

                list_ind = get_arr_ind_from_position(obj_transform.localPosition);
                Debug.Log(obj_transform.localPosition);
                Debug.Log(list_ind);
                GridMaker.tiles[(int)list_ind.x][(int)list_ind.y] = start_obj.gameObject;

                start_obj.localPosition = obj_transform.localPosition;
                obj_transform.localPosition = temp_pos;
    
                start_obj = null;
                check_colwords();
                // print_board();
            }
      }
}

private static Vector2 get_arr_ind_from_position(Vector2 pos){
    return new Vector2((int)pos.x + GridMaker.dim/2, 
    (int)GridMaker.dim/2 - pos.y);
}

private static bool is_swappable(Vector2 pos1, Vector2 pos2){
    if ((Mathf.Abs(pos1.x - pos2.x) == 1 || Mathf.Abs(pos1.y - pos2.y) == 1)
     && (pos1.x == pos2.x || pos1.y == pos2.y)){
        return true;
    }
    return false;
}

private static void print_board(){
    foreach (List<string> item in GridMaker.letters)
    {
        Debug.Log(string.Join("", item));
    }
}



}
