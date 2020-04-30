using System; 
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class PyzymeWordSearch {

    public static Tuple<int, int> get_word_match_index(string str){
        Debug.Log("Matching word: " + str +"--hash count" + GridMaker.word_hash.Count);
        for (int i=0; i < str.Length-2;i++){
            for (int j=3; j<= str.Length-i;j++){
                 Debug.Log(i+ "" + j);
                if (GridMaker.word_hash.Contains(str.Substring(i, j))){
                    Debug.Log(str.Substring(i, j));
                    return Tuple.Create(i, j);
                }
            }
        }
         return Tuple.Create(-1, -1);
    }

}



 // var items = new List<string> { "armed" , "armed", "jazz", "jaws" };
      // var trie = new Trie();
      // trie.InsertRange(items);
      // string s = "jaws";
      // var prefix = trie.Prefix(s);

      // // returns true if the word is found
      // var foundT = prefix.Depth == s.Length && prefix.FindChildNode('$') != null;
      // Debug.Log(foundT);