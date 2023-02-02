using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GrappleHookController : MonoBehaviour
{
    [Header("Sling Variables")]
    public LayerMask slingAttachLayerMask;
    public LayerMask slingAttachLayerMaskTwo;
    public int layerNum;
    public float maxSlingDistance = 20f;
    private float dist = 0f;
    private float halfDist = 0f;
    [Range(0, 100)]
    public int slingPercentageBounce = 25;
    private LineRenderer slingLineRenderer;
    private GameObject slingAnchorPoint;
    private Rigidbody2D slingHingeAnchorRb;
    private SpriteRenderer slingHingeAnchorSprite;
    private SpringJoint2D slingSpringJoint;
    private bool slingAnchorAttached;
    float aimAngle = -3.1f;


    [Header("Player Variables")]
    [Range(0.0f, 1.0f)]
    public float facingDirDistanceFromPlayer = 0.5f;
    [Range(0.0f, 1.0f)]
    public float gravityScale = 0.75f;
    public Rigidbody2D playerRB;
    public Animator playerAnim;
    public Animator UIAnim;
    private Transform dirIndicator;
    private SpriteRenderer dirIndicatorSprite;
    private Vector2 playerPos;
    public GameObject player;

    [Header("Control Options")]
    public bool multiButtonMouseControls;
    public bool leftMouseControl;
    public bool keyboardAndMouseControls;
    public bool keyboardOnlyControls;
    private bool controlTypeDown;
    private bool controlTypeUp;
    private bool keyboardOnly;

    [Header("Other Private")]
    private List<Vector2> slingWrapPositions = new List<Vector2>();
    private bool distanceSet;
    private bool isColliding;
    private Dictionary<Vector2, int> wrapPointsDictionary = new Dictionary<Vector2, int>();
    private float timerTut;
    private bool tutCanShow = true;

    [Header("Public Instantiations")]
    public Button pauseButton;

    public static int score;

    private bool playAnim = true;

    public static bool highScoreAudioPlayed = false;

    void Awake ()
    {

        Cursor.lockState = CursorLockMode.Confined;

        keyboardOnly = false;

        //Variable Declarations
        slingLineRenderer = gameObject.GetComponent<LineRenderer>();

        slingAnchorPoint = GameObject.Find("Sling Anchor Point");
        slingHingeAnchorRb = slingAnchorPoint.GetComponent<Rigidbody2D>();
        slingHingeAnchorSprite = slingAnchorPoint.GetComponent<SpriteRenderer>();

        slingSpringJoint = gameObject.GetComponent<SpringJoint2D>();

        dirIndicator = GameObject.Find("Direction Indicator").GetComponent<Transform>();
        dirIndicatorSprite = dirIndicator.GetComponent<SpriteRenderer>();

        //Other Declarations
        slingSpringJoint.enabled = false;
	    playerPos = transform.position;

        this.gameObject.GetComponent<Rigidbody2D>().gravityScale = gravityScale;

        //Debug.Log(controlTypeDown + "" + controlTypeUp);

        score = 0;
    }

    private void OnDrawGizmos()
    {
        //Debug.Log("works");
        Vector2 origin = transform.position;
        //Handles.color = Color.red;
        //Handles.DrawWireDisc(origin, new Vector3(0, 0, 1), maxSlingDistance);
    }

    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        // Transform polygoncolliderpoints to world space (default is local)
        Dictionary<float, Vector2> distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)), 
            position => polyCollider.transform.TransformPoint(position));

        IEnumerable<KeyValuePair<float, Vector2>> orderedDictionary = distanceDictionary.OrderBy(e => e.Key);

        if (orderedDictionary.Any())
        {
            return orderedDictionary.First().Value;
        }
        else
        {
            return Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update ()
	{

        Debug.Log("Velocity: " + playerRB.velocity.magnitude);

        if (playerRB.velocity.magnitude > 0.05f)
        {
            score += ((int)(playerRB.velocity.magnitude * (Time.deltaTime * 20)));
        }

        //Debug.Log("Velocity: " + score);


        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));

        if (PlayerPrefs.GetInt("TutPlayed") == 0 && !UI_SceneManager.showTut && tutCanShow)
        {

            if (timerTut < 5f)
            {
                timerTut += Time.deltaTime;
            }
            else
            {
                //UI_SceneManager.showTut = true;

                PlayerPrefs.SetInt("TutPlayed", 1);
            }
            
        }

        if (multiButtonMouseControls)
        {
            controlTypeDown = Input.GetMouseButton(0);
            controlTypeUp = Input.GetMouseButton(1);
        }

        if (leftMouseControl)
        {
            controlTypeDown = Input.GetMouseButtonDown(0);
            controlTypeUp = Input.GetMouseButtonUp(0);
        }

        if (keyboardAndMouseControls)
        {
            controlTypeDown = Input.GetKeyDown(KeyCode.Space);
            controlTypeUp = Input.GetKeyUp(KeyCode.Space);
        }

        if (keyboardOnlyControls)
        {
            controlTypeDown = Input.GetKeyDown(KeyCode.Space);
            controlTypeUp = Input.GetKeyUp(KeyCode.Space);

            keyboardOnly = true;
        }

        if (!keyboardOnly)
        {
            Vector3 facingDir = worldMousePosition - transform.position;
            aimAngle = Mathf.Atan2(facingDir.y, facingDir.x);

        }
        else if (keyboardOnly && !slingAnchorAttached)
        {
            Vector3 facingDir = worldMousePosition - transform.position;

            if (aimAngle > 6.25f)
            {
                aimAngle = 0f;
            }
            else
            {
                aimAngle += Time.deltaTime * 6f;
            }

            //Debug.Log(aimAngle);
        }

        this.gameObject.GetComponent<Rigidbody2D>().gravityScale = gravityScale;

        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        //Debug.Log(aimAngle);

        Vector3 aimDir = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        playerPos = transform.position;

        if (!slingAnchorAttached)
        {
            SetFacingDirSpritePosition(aimAngle);
	    }
	    else
        {
            dirIndicatorSprite.enabled = false;

            // Wrap rope around points of colliders if there are raycast collisions between player position and their closest current wrap around collider / angle point.
	        if (slingWrapPositions.Count > 0)
	        {
	            Vector2 lastSlingPoint = slingWrapPositions.Last();
                RaycastHit2D playerToLastHit = Physics2D.Raycast(playerPos, (lastSlingPoint - playerPos).normalized, Vector2.Distance(playerPos, lastSlingPoint) - 0.1f, slingAttachLayerMaskTwo);
                if (playerToLastHit)
                {
                    PolygonCollider2D colliderWithVertices = playerToLastHit.collider as PolygonCollider2D;
                    if (colliderWithVertices != null)
                    {
                        Vector2 closestHitPoint = GetClosestColliderPointFromRaycastHit(playerToLastHit, colliderWithVertices);
                        if (wrapPointsDictionary.ContainsKey(closestHitPoint))
                        {
                            // Reset the rope if it wraps around an 'already wrapped' position.
                            ResetSling();
                            return;
                        }

                        slingWrapPositions.Add(closestHitPoint);
                        wrapPointsDictionary.Add(closestHitPoint, 0);
                        distanceSet = false;
                    }
                }
            }
        }

        UpdateSlingPositions();
        HandleInput(aimDir, controlTypeDown, controlTypeUp);
        HandleSlingUnwrap();
    }

    private void HandleInput(Vector2 aimDirection, bool inputDown, bool inputUp)
    {

        bool name = Input.GetMouseButton(0);

        if (Input.GetMouseButton(0) && slingAnchorAttached)
        {
                if (dist > halfDist)
                {
                    slingSpringJoint.distance = dist / 1.007f;
                    dist = slingSpringJoint.distance;
                }
            


            
            Mathf.Lerp(0f, dist, 0.5f);

            

        }

        if (inputDown && !GameManagerMike.isPause)
        {

            tutCanShow = false;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (EventSystem.current.currentSelectedGameObject.name == "PauseBtn" || EventSystem.current.currentSelectedGameObject.name == "RestartBtn")
                {

                    //Debug.Log("Name: " + EventSystem.current.currentSelectedGameObject.name);

                    return;
                }


                //if (EventSystem.current.currentSelectedGameObject.gameObject != null)
                //{
                //    if (EventSystem.current.currentSelectedGameObject.name == "PauseBtn")
                //    {
                //        Debug.Log("YEAH");
                //        return;
                //    }
                //}
            }

            if (slingAnchorAttached)
            {
                return;
            }

            slingLineRenderer.enabled = true;

            RaycastHit2D hit = Physics2D.Raycast(playerPos, aimDirection, maxSlingDistance, slingAttachLayerMask);

            //Debug.Log("Layer: " + hit.transform.gameObject.layer);


            
            if (hit.collider != null && hit.transform.gameObject.layer == layerNum)
            {
                slingAnchorAttached = true;
                if (!slingWrapPositions.Contains(hit.point))
                {

                    if (slingWrapPositions.Count == 0)
                    {
                        dist = Vector2.Distance(playerPos, hit.point) - (Vector2.Distance(playerPos, hit.point) * (slingPercentageBounce + gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) / 100);
                        slingSpringJoint.distance = (dist);

                        halfDist = dist / 1.75f;

                        AudioManager.attachAudio.Play();

                       //Debug.Log("Distance: " + Mathf.Lerp(0f, 1f, 0.1f));
                        //slingSpringJoint.frequency = 1;
                    }


                    //transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 200f), ForceMode2D.Force);
                    slingWrapPositions.Add(hit.point);
                    wrapPointsDictionary.Add(hit.point, 0);
                    //slingSpringJoint.distance = Vector2.Distance(playerPos, hit.point);
                    slingSpringJoint.enabled = true;
                    //slingHingeAnchorSprite.enabled = true;
                }
            }
            else
            {
                slingLineRenderer.enabled = false;
                slingAnchorAttached = false;
                slingSpringJoint.enabled = false;
            }
        
        }


        if (inputUp && !GameManagerMike.isPause)
        {
            ResetSling();
        }
    }

    private void ResetSling()
    {
        slingSpringJoint.enabled = false;
        slingAnchorAttached = false;
        slingLineRenderer.positionCount = 2;
        slingLineRenderer.SetPosition(0, transform.position);
        slingLineRenderer.SetPosition(1, transform.position);
        slingWrapPositions.Clear();
        wrapPointsDictionary.Clear();
        slingHingeAnchorSprite.enabled = false;
    }
    private void SetFacingDirSpritePosition(float aimAngle)
    {
        if (!dirIndicatorSprite.enabled)
        {
            dirIndicatorSprite.enabled = true;
        }

        float facingSpriteX = transform.position.x + facingDirDistanceFromPlayer * Mathf.Cos(aimAngle);
        float facingSpriteY = transform.position.y + facingDirDistanceFromPlayer * Mathf.Sin(aimAngle);

        Vector3 facingDirSprite = new Vector3(facingSpriteX, facingSpriteY, 0);

        if (!GameManagerMike.isPause)
        {
            dirIndicator.transform.position = facingDirSprite;
        }

        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(mouseScreen);

        dirIndicator.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(player.transform.position.y, player.transform.position.x) * Mathf.Rad2Deg - 90);
    }

    private void UpdateSlingPositions()
    {
        if (slingAnchorAttached)
        {
            slingLineRenderer.positionCount = slingWrapPositions.Count + 1;

            for (int i = slingLineRenderer.positionCount - 1; i >= 0; i--)
            {
                if (i != slingLineRenderer.positionCount - 1) // if not the Last point of line renderer
                {
                    slingLineRenderer.SetPosition(i, slingWrapPositions[i]);
                    
                    // Set the rope anchor to the 2nd to last rope position (where the current hinge/anchor should be) or if only 1 rope position then set that one to anchor point
                    if (i == slingWrapPositions.Count - 1 || slingWrapPositions.Count == 1)
                    {
                        if (slingWrapPositions.Count == 1)
                        {
                            Vector2 slingPos = slingWrapPositions[slingWrapPositions.Count - 1];
                            slingHingeAnchorRb.transform.position = slingPos;
                            if (!distanceSet)
                            {
                                slingSpringJoint.distance = Vector2.Distance(transform.position, slingPos);
                                distanceSet = true;
                            }
                        }
                        else
                        {
                            Vector2 slingPos = slingWrapPositions[slingWrapPositions.Count - 1];
                            slingHingeAnchorRb.transform.position = slingPos;
                            if (!distanceSet)
                            {
                                slingSpringJoint.distance = Vector2.Distance(transform.position, slingPos);
                                distanceSet = true;
                            }
                        }
                    }
                    else if (i - 1 == slingWrapPositions.IndexOf(slingWrapPositions.Last()))
                    {
                        // if the line renderer position we're on is meant for the current anchor/hinge point...
                        Vector2 slingPos = slingWrapPositions.Last();
                        slingHingeAnchorRb.transform.position = slingPos;
                        if (!distanceSet)
                        {
                            slingSpringJoint.distance = Vector2.Distance(transform.position, slingPos);
                            distanceSet = true;
                        }
                    }
                }
                else
                {
                    // Player position
                    slingLineRenderer.SetPosition(i, transform.position);
                }
            }
        }
    }

    private void HandleSlingUnwrap()
    {
        if (slingWrapPositions.Count <= 1)
        {
            return;
        }

        // Hinge = next point up from the player position
        // Anchor = next point up from the Hinge
        // Hinge Angle = Angle between anchor and hinge
        // Player Angle = Angle between anchor and player

        int anchorIndex = slingWrapPositions.Count - 2;
        int hingeIndex = slingWrapPositions.Count - 1;
        Vector2 anchorPosition = slingWrapPositions[anchorIndex];
        Vector2 hingePosition = slingWrapPositions[hingeIndex];
        Vector2 hingeDir = hingePosition - anchorPosition;
        float hingeAngle = Vector2.Angle(anchorPosition, hingeDir);
        Vector2 playerDir = playerPos - anchorPosition;
        float playerAngle = Vector2.Angle(anchorPosition, playerDir);

        if (!wrapPointsDictionary.ContainsKey(hingePosition))
        {
            return;
        }

        if (playerAngle < hingeAngle)
        {
            if (wrapPointsDictionary[hingePosition] == 1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }

            wrapPointsDictionary[hingePosition] = -1;
        }
        else
        {
            if (wrapPointsDictionary[hingePosition] == -1)
            {
                UnwrapRopePosition(anchorIndex, hingeIndex);
                return;
            }
            wrapPointsDictionary[hingePosition] = 1;
        }
    }

    private void UnwrapRopePosition(int anchorIndex, int hingeIndex)
    {
        Vector2 newAnchorPosition = slingWrapPositions[anchorIndex];
        wrapPointsDictionary.Remove(slingWrapPositions[hingeIndex]);
        slingWrapPositions.RemoveAt(hingeIndex);

        slingHingeAnchorRb.transform.position = newAnchorPosition;
        distanceSet = false;

        // Set new sling distance joint distance for anchor position if not yet set.
        if (distanceSet)
        {
            return;
        }
        slingSpringJoint.distance = Vector2.Distance(playerPos, newAnchorPosition) - (Vector2.Distance(playerPos, newAnchorPosition) * (slingPercentageBounce + gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) / 100);
        distanceSet = true;
    }

    void OnTriggerStay2D(Collider2D colliderStay)
    {
        isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D colliderOnExit)
    {
        isColliding = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.bounceAudio.Play();

        GrappleHookController.highScoreAudioPlayed = false;

        if (PlayerPrefs.GetInt("HighScore") < score)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        score = 0;

        if (playAnim)
        {
            playerAnim.Play("Collide");
            UIAnim.Play("Collide UI");

            StartCoroutine(Reset());
        }
    }

    private IEnumerator Reset()
    {
        playAnim = false;
        yield return new WaitForSeconds(0.5f);
        playAnim = true;
    }
}
