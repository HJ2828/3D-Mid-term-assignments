using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject GateLeft;
    public GameObject GateRight;
    public GameObject OpenPanel;

    public GameObject Player;
    public GameObject player_key;

    private bool openCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool ps = PlayerScale();

        if (ps == true && player_key.activeSelf == true)
        {
            openCheck = true;
        }
        else {
            openCheck = false;
        }
            
    }

    private bool PlayerScale()
    {
        float px = Player.transform.localScale.x;
        float py = Player.transform.localScale.y;
        float pz = Player.transform.localScale.z;

        if (px <= 0.5f && py <= 0.5f && pz <= 0.5f)     // 캐릭터 크기가 (0.5, 0.5, 0.5) 이하면
        {
            return true;
        }
        else
            return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(openCheck == true)
            {
                GateLeft.transform.rotation = Quaternion.Euler(0, -90, 0);
                GateRight.transform.rotation = Quaternion.Euler(0, 90, 0);

                player_key.SetActive(false);
            }
        }
    }
}
