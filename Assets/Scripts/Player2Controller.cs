
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Blockade 1976 v2021.02.24
//
// 2021.02.15
//

public class Player2Controller : MonoBehaviour
{
    public static Player2Controller player2Controller;

    // list of all the player body transforms generated
    private List<Transform> playerBodyTransformList;

    // holds the relevant sprite for the current player body transform
    private Sprite sprite;

    // speed
    private const float MAXIMUM_SPEED = 0.05f;
    private const float MINIMUM_SPEED = 0.5f;

    // directions
    private const int UP = 0;
    private const int DOWN = 1;
    private const int RIGHT = 2;
    private const int LEFT = 3;

    // direction table: up, right, down, left
    List<Vector2Int> directions = new List<Vector2Int>()
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };

    private int direction;

    private Vector2Int gridPosition_snake_xy;

    private float speed;

    [HideInInspector] public int playerBodySize;

    [HideInInspector] public List<Vector2Int> playerBodyPositionList_segments;

    private float gridMoveTimer;

    private bool playerInitialised;
    private bool buildWall;



    private void Awake()
    {
        player2Controller = this;
    }


    void Update()
    {
        if (GameController.gameController.canPlay)
        {
            if (!GameController.gameController.crashed)
            {
                RunMoveTimer();

                KeyboardController();

                UpdateSnakeBodySprites();
            }
        }
    }


    public void Initialise()
    {
        playerInitialised = false;

        playerBodyPositionList_segments = new List<Vector2Int>();

        playerBodyTransformList = new List<Transform>();

        playerBodySize = 1;

        gridPosition_snake_xy = new Vector2Int(25, 1);

        playerBodyPositionList_segments.Add(gridPosition_snake_xy);

        if (playerBodySize > 1)
        {
            for (int i = 1; i < playerBodySize; i++)
            {
                playerBodyPositionList_segments.Add(new Vector2Int(gridPosition_snake_xy.x, gridPosition_snake_xy.y - i));
            }
        }

        direction = UP;

        UpdateSnakeBodySprites();

        speed = MINIMUM_SPEED;

        playerInitialised = true;
    }


    private void KeyboardController()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
    }


    private void MoveUp()
    {
        if (direction != DOWN)
        {
            direction = UP;
        }
    }


    private void MoveDown()
    {
        if (direction != UP)
        {
            direction = DOWN;
        }
    }


    private void MoveLeft()
    {
        if (direction != RIGHT)
        {
            direction = LEFT;
        }
    }


    private void MoveRight()
    {
        if (direction != LEFT)
        {
            direction = RIGHT;
        }
    }


    private void RunMoveTimer()
    {
        gridMoveTimer -= Time.deltaTime;

        if (gridMoveTimer < 0)
        {
            gridMoveTimer = speed;

            MoveSnake();
        }
    }


    private Vector2Int GetNextMove()
    {
        Vector2Int nextMove = new Vector2Int(gridPosition_snake_xy.x + directions[direction][0], gridPosition_snake_xy.y + directions[direction][1]);

        return nextMove;
    }


    private void MoveSnake()
    {
        // check for collision
        // get the coordinates of the next move
        Vector2Int nextMove = GetNextMove();

        int nx = nextMove.x;
        int ny = nextMove.y;

        // collision with playfield wall
        if (nx < 0 || nx > GameController.GRID_WIDTH || ny < 0 || ny > GameController.GRID_HEIGHT)
        {
            Crashed();
        }

        // collision with the player wall
        for (int i = 0; i < playerBodyPositionList_segments.Count; i++)
        {
            int sx = playerBodyPositionList_segments[i].x;
            int sy = playerBodyPositionList_segments[i].y;

            // hit player wall
            if (nx == sx && ny == sy)
            {
                Crashed();
            }
        }

        for (int i = 0; i < Player1Controller.player1Controller.playerBodyPositionList_segments.Count; i++)
        {
            int sx = Player1Controller.player1Controller.playerBodyPositionList_segments[i].x;
            int sy = Player1Controller.player1Controller.playerBodyPositionList_segments[i].y;

            // hit player wall
            if (nx == sx && ny == sy)
            {
                Crashed();
            }
        }

        // get new position
        gridPosition_snake_xy += directions[direction];

        playerBodyPositionList_segments.Insert(0, gridPosition_snake_xy);

        buildWall = true;

        if (buildWall)
        {
            // increase body size
            playerBodySize += 1;

            if (speed > MAXIMUM_SPEED)
            {
                speed -= 0.001f;
            }
        }


        // check size of snake body
        if (playerBodyPositionList_segments.Count >= playerBodySize + 1)
        {
            playerBodyPositionList_segments.RemoveAt(playerBodyPositionList_segments.Count - 1);
        }


        // move body to new position
        for (int i = 0; i < playerBodyTransformList.Count; i++)
        {
            Vector2 snakeBodyPosition = new Vector2(playerBodyPositionList_segments[i].x, playerBodyPositionList_segments[i].y);

            playerBodyTransformList[i].position = snakeBodyPosition;
        }
    }


    private void Crashed()
    {
        GameController.gameController.UpdatePlayer1Score(1);

        GameController.gameController.crashed = true;
    }


    public List<Vector2Int> ReturnSnakeGridPosition()
    {
        // head position
        List<Vector2Int> snakeGridPosition = new List<Vector2Int>() { gridPosition_snake_xy };

        // body positions
        snakeGridPosition.AddRange(playerBodyPositionList_segments);

        return snakeGridPosition;
    }


    private void UpdateSnakeBodySprites()
    {
        if (playerBodySize > 1)
        {
            UpdateMultipleSprites();
        }

        else
        {
            int i = 0;

            UpdateBody(i);
        }
    }


    private void UpdateMultipleSprites()
    {
        // loop over every snake segment
        for (int i = 0; i < playerBodyPositionList_segments.Count; i++)
        {
            UpdateBody(i);
        }
    }


    private void UpdateBody(int i)
    {
        Vector2Int currentBodySegment = playerBodyPositionList_segments[i];

        if (playerBodySize > 1)
        {
            // head
            if (i == 0)
            {
                UpdateSnakeHead(i, currentBodySegment);
            }


            // tail
            else if (i == playerBodyPositionList_segments.Count - 1)
            {
                // previous segment
                Vector2Int previousBodySegment = playerBodyPositionList_segments[i - 1];

                // tail down
                if (previousBodySegment.y < currentBodySegment.y)
                {
                    sprite = GameAssets.gameAsset.wallVerticalSprite;
                }

                // tail right
                else if (previousBodySegment.x > currentBodySegment.x)
                {
                    sprite = GameAssets.gameAsset.wallHorizontalSprite;
                }

                // tail up
                else if (previousBodySegment.y > currentBodySegment.y)
                {
                    sprite = GameAssets.gameAsset.wallVerticalSprite;
                }

                // tail left
                else if (previousBodySegment.x < currentBodySegment.x)
                {
                    sprite = GameAssets.gameAsset.wallHorizontalSprite;
                }
            }

            // body
            else
            {
                // previous segment
                Vector2Int previousBodySegment = playerBodyPositionList_segments[i - 1];

                // next segment
                Vector2Int nextBodySegment = playerBodyPositionList_segments[i + 1];

                // body horizontal
                if (previousBodySegment.x < currentBodySegment.x && nextBodySegment.x > currentBodySegment.x || nextBodySegment.x < currentBodySegment.x && previousBodySegment.x > currentBodySegment.x)
                {
                    sprite = GameAssets.gameAsset.wallHorizontalSprite;
                }

                // moving up - body bottom right
                else if (previousBodySegment.x < currentBodySegment.x && nextBodySegment.y > currentBodySegment.y || nextBodySegment.x < currentBodySegment.x && previousBodySegment.y > currentBodySegment.y)
                {
                    sprite = GameAssets.gameAsset.wallBottomRightSprite;
                }

                // body vertical
                else if (previousBodySegment.y < currentBodySegment.y && nextBodySegment.y > currentBodySegment.y || nextBodySegment.y < currentBodySegment.y && previousBodySegment.y > currentBodySegment.y)
                {
                    sprite = GameAssets.gameAsset.wallVerticalSprite;
                }

                // moving left - body top right
                else if (previousBodySegment.y < currentBodySegment.y && nextBodySegment.x < currentBodySegment.x || nextBodySegment.y < currentBodySegment.y && previousBodySegment.x < currentBodySegment.x)
                {
                    sprite = GameAssets.gameAsset.wallTopRightSprite;
                }

                // moving down - body top left
                else if (previousBodySegment.x > currentBodySegment.x && nextBodySegment.y < currentBodySegment.y || nextBodySegment.x > currentBodySegment.x && previousBodySegment.y < currentBodySegment.y)
                {
                    sprite = GameAssets.gameAsset.wallTopLeftSprite;
                }

                // moving right - body bottom left
                else if (previousBodySegment.y > currentBodySegment.y && nextBodySegment.x > currentBodySegment.x || nextBodySegment.y > currentBodySegment.y && previousBodySegment.x > currentBodySegment.x)
                {
                    sprite = GameAssets.gameAsset.wallBottomLeftSprite;
                }
            }


            RenderState(i);
        }

        else
        {
            UpdateSnakeHead(i, currentBodySegment);

            RenderState(i);
        }
    }


    private void UpdateSnakeHead(int i, Vector2Int currentBodySegment)
    {
        if (playerBodySize > 1)
        {
            Vector2Int nseg = playerBodyPositionList_segments[i + 1]; // Next segment

            // head down
            if (currentBodySegment.y < nseg.y)
            {
                sprite = GameAssets.gameAsset.player2HeadDownSprite;
            }

            // head right
            else if (currentBodySegment.x > nseg.x)
            {
                sprite = GameAssets.gameAsset.player2HeadRightSprite;
            }

            // head up
            else if (currentBodySegment.y > nseg.y)
            {
                sprite = GameAssets.gameAsset.player2HeadUpSprite;
            }

            // head left
            else if (currentBodySegment.x < nseg.x)
            {
                sprite = GameAssets.gameAsset.player2HeadLeftSprite;
            }
        }

        else
        {
            if (direction == DOWN)
            {
                sprite = GameAssets.gameAsset.player2HeadDownSprite;
            }

            if (direction == RIGHT)
            {
                sprite = GameAssets.gameAsset.player2HeadRightSprite;
            }

            if (direction == UP)
            {
                sprite = GameAssets.gameAsset.player2HeadUpSprite;
            }

            if (direction == LEFT)
            {
                sprite = GameAssets.gameAsset.player2HeadLeftSprite;
            }
        }
    }


    private void RenderState(int i)
    {
        if (!playerInitialised || buildWall)
        {
            DrawNewSnakeSprite(i, sprite);

            buildWall = false;
        }

        else
        {
            UpdateCurrentSnakeSprite(i, sprite);
        }
    }


    private void DrawNewSnakeSprite(int i, Sprite sprite)
    {
        GameObject snakeBodyGameObject = Player2ObjectPooler.player2ObjectPooler.GetPooledObject();

        snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = sprite;

        snakeBodyGameObject.transform.position = new Vector2(playerBodyPositionList_segments[i].x, playerBodyPositionList_segments[i].y);

        playerBodyTransformList.Add(snakeBodyGameObject.transform);

        snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -playerBodyTransformList.Count;

        snakeBodyGameObject.SetActive(true);
    }


    private void UpdateCurrentSnakeSprite(int i, Sprite sprite)
    {
        Transform snakeBodyPart = playerBodyTransformList[i].transform;

        snakeBodyPart.GetComponent<SpriteRenderer>().sprite = sprite;

        snakeBodyPart.position = new Vector2(playerBodyPositionList_segments[i].x, playerBodyPositionList_segments[i].y);
    }


} // end of class
