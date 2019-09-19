using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesObjectCaching : MonoBehaviour
{
    // 캐싱할 무기 이미지들
    private static Dictionary<string ,Sprite> weaponSprites;

    // Start is called before the first frame update
    void Start()
    {
        CachingWeaponSprites();
    }

    private static void CachingWeaponSprites()
    {
        weaponSprites = new Dictionary<string, Sprite>();
        Sprite[] temps = Resources.LoadAll<Sprite>("Sprites/Object");
        foreach (Sprite temp in temps)
            weaponSprites.Add(temp.name, temp);
    }

    public static Sprite GetWeaponSprite(string weaponName)
    {
        return weaponSprites[weaponName];
    }
}
