using Flag;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Camera_controls
{
    public class PanZoom : MonoBehaviour
    {
        [SerializeField] private Arrow_Controller controller;
        [SerializeField] private GameObject playerUnit;

        Vector3 touchStart;
        public float zoomOutMin = 1;
        public float panZoomOutMax = 7.1f;

        private Camera cam;
        public bool drag, canDrag = true;

        public LandChunk selectedLand, moveToLand;

        private void Start()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            if (!canDrag) return;

            if (Input.GetMouseButtonDown(0))
            {
                touchStart = cam.ScreenToWorldPoint(Input.mousePosition);

                var hitInfo = Physics2D.Raycast(touchStart, Vector3.up, Mathf.Infinity);
                var selectedLand = hitInfo.collider != null ? hitInfo.collider.GetComponent<LandChunk>() : null;
                
                if (selectedLand && selectedLand.unitAvailable)
                {
                    if (selectedLand.unitPower > 0)
                    {
                        if (selectedLand.unitClaimed == GameData.MapData.UnitClaimed.Player)
                        {
                            if (selectedLand.inCombat == false)
                            {
                                controller.points[0].transform.position = Physics2D.Raycast(touchStart, Vector3.up, Mathf.Infinity).collider.transform.position;
                                this.selectedLand = selectedLand;
                                drag = true;
                            }
                        }
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
                var hitInfo = Physics2D.Raycast(touchStart, Vector3.up, Mathf.Infinity);

                if (drag && hitInfo.collider && hitInfo.collider.GetComponent<LandChunk>())
                {
                    touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
                    controller.points[1].transform.position = Physics2D.Raycast(touchStart, Vector3.up, Mathf.Infinity).collider.transform.position;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
                var hitInfo = Physics2D.Raycast(touchStart, Vector3.up, Mathf.Infinity);
                var landChunk = hitInfo.collider != null ? hitInfo.collider.GetComponent<LandChunk>() : null;

                if (drag && landChunk)
                {
                    if (selectedLand.inCombat) return;

                    moveToLand = hitInfo.collider.GetComponent<LandChunk>();
                    //Check if we not hovering over the same land
                    if (moveToLand != selectedLand)
                    {
                        SpawnPlayerUnit(selectedLand, moveToLand);
                        selectedLand.unitAvailable = false;
                    }
                }

                //Makes the arrow hidden
                controller.points[1].transform.position = controller.points[0].transform.position;
                drag = false;
            }

            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMaginitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMaginitude - prevMagnitude;
                Zoom(difference * 0.01f);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += direction;
            }

            Zoom(Input.GetAxis("Mouse ScrollWheel"));

            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SceneManager.LoadScene(0, LoadSceneMode.Single);
                }
            }
        }

        void Zoom(float increment)
        {
            cam.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, panZoomOutMax);
        }
    
        public void SpawnPlayerUnit(LandChunk landOrigin, LandChunk landToAttack)
        {
            Vector3 spawnPosition = new Vector3(landOrigin.transform.position.x + Random.Range(-.3f, .3f),
                landOrigin.transform.position.y + Random.Range(-.3f, .3f), -0.5f);
            var unit = Instantiate(playerUnit, spawnPosition, Quaternion.identity);
            var unitAgent = unit.GetComponent<UnitAgent>();
            unitAgent.Initialize(landToAttack, landOrigin);
            //landOrigin.unitPower--;
            //landOrigin.unitCountText.text = $"Power: {landOrigin.unitPower}";
        }
    }
}
