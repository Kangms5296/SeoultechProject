using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShowcase : MonoBehaviour {

    public GameObject[] weapons;
    public GameObject cam;
    public int currentWeapon;
    private Vector3 camOffset;
    private Vector3 weaponOffset = new Vector3(1,1,0);
    public Quaternion targetRotation = new Quaternion(-0.1767767f, 0.3061862f, 0.1767767f, 0.9185587f);
    public UnityEngine.UI.Text weaponNameUI;

	// Use this for initialization
	void Start () {
        PositionWeapons();
        camOffset = cam.transform.position - weapons[0].transform.position;
    }


    // Create GameObject array and Sets Position of weapons.
    void PositionWeapons()
    {
        
        weapons = new GameObject[transform.childCount];

        for(int i=0; i< transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i).gameObject;
        }

        for(int i=0; i< weapons.Length; i++)
        {
            //Sets position for Weapon based on its index in array.
            weapons[i].transform.position = i * weaponOffset;
        }

    }
	
	// Update is called once per frame
	void Update () {

        //Move to Next Weapon.
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentWeapon++;
        }

        //Move to Previous Weapon.
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentWeapon--;
        }

        //Clamps currentWeapon value between 0 and max Number of weapons.
        currentWeapon = Mathf.Clamp(currentWeapon, 0, weapons.Length-1);

        //Sets to ui Text to current weapons Name.
        weaponNameUI.text = weapons[currentWeapon].name;

        //Target Position for Camera.
        Vector3 targetPosition = weapons[currentWeapon].transform.position + camOffset;

        //Moves Camera.
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, Time.deltaTime * 5);

    }

    // Fixed update for lerping Rotation of weapons.
    void FixedUpdate()
    {

        for (int i = 0; i < weapons.Length; i++)
        {
            if (i != currentWeapon)
            {
                weapons[i].transform.rotation = Quaternion.Lerp(weapons[i].transform.rotation, Quaternion.identity, Time.fixedDeltaTime*8);
            }
            else
            {
                weapons[currentWeapon].transform.rotation = Quaternion.Lerp(weapons[i].transform.rotation, targetRotation, Time.fixedDeltaTime*6);
            }
        }

    }
}
