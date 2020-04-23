using System; 
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class PyzymeWordSearch{

    public static HashSet<string> get_word_hash(){
        string path = "Assets/Resources/words.txt";
        HashSet<string> mySet = new HashSet<string>(); 
        var stream = new StreamReader(@path);

        while (!stream.EndOfStream)
            mySet.Add(stream.ReadLine());

        return mySet;
    }

    public static Tuple<int, int> get_word_match_index(string str){
        for (int i=0; i < str.Length;i++){
            for (int j=3; j< str.Length-i;j++){
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