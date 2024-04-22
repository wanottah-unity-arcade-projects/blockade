

using UnityEngine;

//
// Blockade 1976 v2021.02.24
//
// 2021.02.15
//

public class GameAssets : MonoBehaviour
{
    public static GameAssets gameAsset;

    public Sprite player1HeadLeftSprite;
    public Sprite player1HeadRightSprite;
    public Sprite player1HeadUpSprite;
    public Sprite player1HeadDownSprite;

    public Sprite player2HeadLeftSprite;
    public Sprite player2HeadRightSprite;
    public Sprite player2HeadUpSprite;
    public Sprite player2HeadDownSprite;

    public Sprite wallHorizontalSprite;
    public Sprite wallVerticalSprite;
    public Sprite wallTopLeftSprite;
    public Sprite wallTopRightSprite;
    public Sprite wallBottomLeftSprite;
    public Sprite wallBottomRightSprite;

    public Sprite crashedSprite;


    private void Awake()
    {
        gameAsset = this;
    }


} // end of class
