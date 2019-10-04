using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairScript : MonoBehaviour
{
    RectTransform rect;
    Animator anim;

    Vector2 pos = Vector2.zero;
    float posX = 0;
    float posY = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();

        gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        posX = posX + Input.GetAxis("Mouse X") * 20;
        posY = posY + Input.GetAxis("Mouse Y") * 20;

        pos = Vector2.Lerp(pos, new Vector2(posX, posY), Time.deltaTime * 20);

        rect.anchoredPosition = pos;
    }

    public void Init(Vector3 playerForward)
    {
        pos = new Vector2(playerForward.x * 1920, playerForward.z * 1080) * 0.3f;
        posX = pos.x;
        posY = pos.y;

        gameObject.SetActive(true);
    }

    public void Destroy()
    {
        anim.SetTrigger("Destroy");
    }

    public void End()
    {
        gameObject.SetActive(false);
    }
   
    public Vector3 CrossHairWorldPosition()
    {
        float x = rect.anchoredPosition.x + 960;
        float y = rect.anchoredPosition.y + 540;
        Vector2 mousePos = new Vector2(x, y);

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 100f))
        {
            if (Physics.Raycast(ray, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")))
                return hitInfo.point;
        }
        return Vector3.zero;
    }
}
