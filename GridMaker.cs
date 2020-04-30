using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using System.IO;
using UnityEngine.Networking;

// custom scripts
using static PyzymeInput;
using static PyzymeGameplay;
using static Trie;
using static PyzymeWordSearch;

public class GridMaker : MonoBehaviour
{
  public static HashSet<string> word_hash;
  public GameObject [] tile_prefab;
  public GameObject [] solved_words;
  public GameObject game_board;
  public GameObject loading_screen;
  public static List<List<GameObject>> tiles;
  public static List<List<string>> letters;
  private static System.Random rnd = new System.Random();
  private string [] alphabets;
  public static int dim = 9; // gameboard dimension, square
    // Start is called before the first frame update

    void Start()
    {
      alphabets = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z".Split(' ');
      get_word_hash();
      //sets up the grid of letters
      make_grid(); 
      //adjusts the scale of gameboard to fit the screen
      adjust_scale(game_board.transform, new Vector2(0.7f, 0.7f));
      get_letters_onboard();  // update letter array with latest changes
      check_words();
    }

    // Update is called once per frame
    void Update()
    {
      // get the transform of touched or clicked object
      (string state, Transform obj_transform) = hit_object_transform();

      // swap tiles as required
      // check words as tiles are swapped, does latest swap lead to a match?
      swap_tiles(state, obj_transform, check_words);
    }


    public void check_words(){
      check_col_words();
      check_row_words();
    }


    public void check_col_words(){
      for (int i=0; i< tiles.Count; i++){
        // start and end index (exclusive) of matched word in the string
        get_letters_onboard();  // update letter array with latest changes
        (int i_start, int count) = get_word_match_index(string.Join("", letters[i]));
        if (i_start != -1){
          display_solved_words(string.Join("", letters[i]).Substring(i_start, count));
          remove_and_fill_matched_words(i, i_start, count);
          check_words(); // optimize this to check only the columns and rows affected use overloading?
        }
      }
    }

    public void check_row_words(){
      for (int row = 0; row < dim; row++)
      {
        List<string> row_letters = new List<string>();
        for (int i = 0; i < letters.Count; i++){
          row_letters.Add(letters[i][row]);
        }
        
        (int i_start, int count) = get_word_match_index(string.Join("", row_letters));
        if (i_start != -1){
          display_solved_words(string.Join("", row_letters).Substring(i_start, count));
          for (int col = i_start; col < i_start+count; col++)
          {
            get_letters_onboard(); // update letter array with latest changes
            Debug.Log("col: "+col);
            remove_and_fill_matched_words(col, row, 1);
            check_words();// optimize this to check only the columns and rows affected use overloading?
          }
        }
      }
    }

    public void display_solved_words(String word){
      solved_words[1].GetComponent<TextMeshPro>().text = solved_words[0].GetComponent<TextMeshPro>().text;
      solved_words[0].GetComponent<TextMeshPro>().text = word;
    }
    public void make_grid(){
      tiles = new List<List<GameObject>>();
      // beginning and end of the range in for loop ...
      // this equally distributes tiles around the board
      int range_start = -(int)dim/2;
      int range_end = ((int)dim/2) + 1;

      // adjust the range offset for array indices
      int array_ind = (int)dim/2;
      for (int i=range_start; i < range_end; i++){
          List<GameObject> row = new List<GameObject>();
          for (int j=range_start; j < range_end; j++){

            // instantiated object
            GameObject temp_tile = Instantiate(tile_prefab[rnd.Next(
              tile_prefab.Length)], new Vector2(i, j), Quaternion.identity, game_board.transform);

            // pick a random letter to assign to textMesh
            string letter = alphabets[rnd.Next(alphabets.Length)];
            temp_tile.GetComponentInChildren<TextMeshPro>().text = letter;
            row.Add(temp_tile);
        }
        // because the objects are added bottomup and the words are read topdown
        row.Reverse();
        tiles.Add(row);
      }

    }


    public void get_letters_onboard(){
      // array to hold spawned letter tile transforms
      
      letters = new List<List<string>>();
      
      foreach (List<GameObject> obj_list in tiles ){
        List<string> temp_list = new List<string>();
        foreach (GameObject obj in obj_list)
        {
            TextMeshPro temp_text =  obj.GetComponentInChildren<TextMeshPro>();
            temp_list.Add(temp_text.text);
        }
        letters.Add(temp_list);
      }
    }

    public void remove_and_fill_matched_words(int col, int r_start, int count){
      Debug.Log(string.Join("", letters[col]));
      // matched letter tiles, that needs to be refreshed with new letters and position
      List<GameObject> to_switch_letter_and_pos = tiles[col].GetRange(r_start, count);
      tiles[col].RemoveRange(r_start, count);
      Debug.Log(string.Join("", letters[col].GetRange(r_start, count)));
      Debug.Log(tiles[col].Count);
      // the letter tiles that needs to fall down to the above space
      List<GameObject> to_switch_pos = tiles[col].GetRange(r_start, tiles[col].Count - r_start);
      tiles[col].RemoveRange(r_start, tiles[col].Count - r_start);

      if (!to_switch_pos.Any()){ //end of the array, just switch letters
        to_switch_letter_and_pos = switch_letters(to_switch_letter_and_pos, col);

        // add the processed tiles back to the list of lists
        tiles[col].AddRange(to_switch_letter_and_pos);

      }else{ // the tiles[col] is empty -> a bug here
        switch_position(to_switch_pos, col);
        to_switch_letter_and_pos = switch_letters(to_switch_letter_and_pos, col);
        switch_position(to_switch_letter_and_pos, col);
      }

    }

    public List<GameObject>  switch_letters(List<GameObject> tiles_obj, int col){
      // rnd = new System.Random();
      foreach (GameObject item in tiles_obj)
        {
          string letter = alphabets[rnd.Next(alphabets.Length)];
          item.GetComponentInChildren<TextMeshPro>().text = letter;
        }
       return tiles_obj; 
    }

    public void switch_position(List<GameObject> tiles_obj, int col){
      Debug.Log(tiles[col].Count);
      for(int i=0;i<tiles_obj.Count;i++){
        Vector2 temp_pos = get_ref_position(col);
        // Debug.Log(temp_pos);
        temp_pos.y -= i+1;
        // Debug.Log(temp_pos);
        tiles_obj[i].transform.localPosition = temp_pos;
      }
      tiles[col].AddRange(tiles_obj);
    }


    public Vector2 get_ref_position(int col){
      float x = col - (int)(dim/2); // convert list index to position x (col)
      Vector2 temp_pos = new Vector2(x, 5f); // 5 because we subtract 1 before setting position
      if (tiles[col].Any()){
        GameObject ref_obj = tiles[col][tiles[col].Count-1];
        temp_pos = ref_obj.transform.localPosition;
      }
      return temp_pos;
    }

    public void random_vanish(){
      HashSet <GameObject> uniq = new HashSet<GameObject>();
      while (uniq.Count < 5){
        int rnd_x= rnd.Next(5);
        int rnd_y= rnd.Next(5);
        uniq.Add(tiles[rnd_x][rnd_y]);
        tiles[rnd_x][rnd_y].gameObject.SetActive(false);
      }
    }

    public void adjust_scale(Transform obj, Vector2 obj_scale){
      obj.localScale = obj_scale;
    }

    public void get_word_hash(){
      word_hash = new HashSet<string>();
      #if UNITY_EDITOR
      string filePath = Path.Combine (Application.streamingAssetsPath, "words.txt");
      var stream = new StreamReader(@filePath);

      while (!stream.EndOfStream)
          word_hash.Add(stream.ReadLine());


      #elif UNITY_IOS
      
      string filePath = Path.Combine (Application.streamingAssetsPath + "/Raw", "words.txt");
      var stream = new StreamReader(@filePath);

      while (!stream.EndOfStream)
          word_hash.Add(stream.ReadLine());


      #elif UNITY_ANDROID
      StartCoroutine("get_word_hash_android", loading_screen);
      #endif

      Debug.Log("word hash length: " + word_hash.Count);
  }


    public IEnumerator get_word_hash_android(GameObject loading_screen){
      loading_screen.SetActive(true);
      string filePath = Path.Combine (Application.streamingAssetsPath, "words.txt");
      UnityWebRequest www = UnityWebRequest.Get(filePath);
      yield return www.SendWebRequest();
      if (www.isNetworkError || www.isHttpError){
          Debug.Log(www.error);
      }else{
          string [] word_arr = www.downloadHandler.text.Split('\n').Select(p => p.Trim()).ToArray();
          word_hash = new HashSet<string>(word_arr);
          // word_hash = new HashSet<string>(word_arr);
          Debug.Log("word hash length: " + word_hash.Count);
          check_words();
          loading_screen.SetActive(false);
        }
      
  }
}


      // var coords = new(int x, int y)[]{

      //   (1, 0), (0, 1), (-1, 0), (0, -1)
      // };

              // foreach (int index in Enumerable.Range( 1, 7 ))
        // {
        // Debug.Log(index);
        // }