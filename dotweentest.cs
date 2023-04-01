using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyDotween;

public class dotweentest : MonoBehaviour
{
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Doooooooooooooooostart");

            MyDotween.Sequence sq = new MyDotween.Sequence();
            sq.Append(new MyDotween.Tween(this.gameObject, new Vector3(10, 0, 0), 2, Dotween.Ease.Linear))
                .Append(new MyDotween.Tween(this.gameObject, new Vector3(10, 0, 10), 2, Dotween.Ease.Linear))
                .Append(new MyDotween.Tween(this.gameObject, new Vector3(10, 0, 0), 2, Dotween.Ease.Linear))
                .Join(new MyDotween.Tween(obj, obj.transform.position + new Vector3(10, 0, 0), 2, Dotween.Ease.Linear))
                .Prepend(new MyDotween.Tween(obj, obj.transform.position + new Vector3(-10, 0, 0), 2, Dotween.Ease.Linear));

            sq.Start();

            //Vector3 targetpos = transform.position + (-transform.right * speed * Time.deltaTime);


            //Dotween dotween = new Dotween();
            //dotween.SetEase(Dotween.Ease.easeInCubic);
            //dotween.DoMove(this.gameObject, new Vector3(20, 0, 0), 3);
        }
    }
}