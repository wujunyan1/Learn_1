using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public interface SaveLoadInterface
{
    void Save(BinaryWriter writer);

    IEnumerator Load(BinaryReader reader);
}
