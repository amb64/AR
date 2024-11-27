using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils.Datums;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public ARTrackedImageManager trackedImageManager;

    public string[] imageNames = {"1_aloy", "1_ff7", "1_playstation", "2_burger", "2_cloud", "2_cloud2", "3_pc", "3_tree", "4_game", "4_turing"};

    private List<string> bodyContent = new List<string>(){"I'm James, a first year. I really liked Object Oriented Programming. You get to code in C# and learn a lot of skills that you'll definitely use as a foundation for coding across the course. We got to make small games like minesweeper and a card game, which I found really fun.\n\nBy the way, I've got some spare RAM for you! Why don't you use this to build your own computer?", 
    "Hi, I'm Meghan! I'm a third year. I'm working on my dissertation, which is about mental health representation in video games. You get to do whatever you want with your dissertation, as long as you make some kind of software as part of it. I chose something that's important to me and super fun!\n\nI've also got an old CPU to give to you. Maybe you can put it to good use?", 
    "My name's Amy. I'm a 2nd year working on my Team Software Engineering project. I'm working in a group with 5 other people to make a program to teach 1st year students about sorting algorithms. I thought I'd hate group projects, but everyone is just as motivated as I am!\n\nI heard you were trying to build your own PC! Well, I've got a motherboard that might help with that!", 
    "I'm Eshaal. I'm a 2nd year games computing student. At the moment I'm doing a group project as part of Concept Development. I love this module as it's not all about coding, and I get to show my passion for art in the graphics of the game we're making!\n\nI've got this power supply if you'd like to take it.", 
    "My name is Manpreet. I love coding, so during Advanced Programming Paradigms in my 2nd year, I got to learn how to make much more complex programs in C++. I also learnt a lot about how to make my code run faster and be more efficient, which is helping a lot in my 3rd year!\n\nYou're building a PC, right? If it doesn't stay cool, it'll overheat! Take these fans!", 
    "Yo, I'm Ellie. I'm a 3rd year studying games computing. I liked studying Artificial Intelligence in my 2nd year. We got to code an AI to make the game 'snake' play itself. It was super challenging, but really cool! It's so fun to try and beat the AI at the game, although I haven't been able to yet...\n\nBy the way, I've got a GPU for you here. It's worth a lot, so keep it safe!", 
    "I'm Alex! Im a 2nd year studying computer science. I loved the User Experience Design module, as you don't have to do any coding! You get to come up with a cool idea and make an interface design for it. It's a great break between all of the other tricky coding modules, and is really important stuff!\n\nYou're building a PC, aren't you? Well, I heard you're missing a hard drive- luckily I've got one for you!", 
    "The name's Reece. You know, computer science isn't just all about languages like C++ and C#. You also get to make a database in Scalable Database Systems, which is in SQL. I liked this module as it made me think about security, and practical business applications of the skills I've learnt.\n\nI got a new monitor recently, so you could take my old one if you'd like.", 
    "Hey there! I'm Zeke. I'm a first year. I didn't do computer science at GCSE or college, so I felt quite behind when I got to uni. But we learn everything from the basics in Programming Fundamentals! I'm all caught up to my classmates now, nothing can stop me from growing!\n\nSomeone handed me this keyboard and mouse to give to you. Here you go!", 
    "I'm Andrew. I took a gap year before coming to uni, so I felt a bit left out. But everyone is really approachable. I was able to make friends in my first year thanks to the Problem Solving group project. Not only did I meet my best friends, but I also had so much fun. This module was one of the highlights of the course!\n\nYou're missing a case for the PC you're building? It's your lucky day, I've got one for you!"};

    public string[] charaNames = {"James", "Meghan", "Amy", "Eshaal", "Manpreet", "Ellie", "Alex", "Reece", "Zeke", "Andrew"};

    public string[] parts = {"RAM", "CPU", "Motherboard", "Power Supply", "Fans", "GPU", "Hard drive", "Monitor", "Keyboard and Mouse", "Case"};

    public List<Sprite> images = new List<Sprite>();

    public List<Sprite> posterImages = new List<Sprite>();

    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI titleText;
    public Image objectImage;

    public TextMeshProUGUI roomText;

    public Image posterImage;

    public GameObject InfoScreen;
    public GameObject MainScreenMenu;
    public GameObject ARSystem;
    public GameObject WinScreen;
    public GameObject GuideButton1;
    public GameObject GuideButton2;
    public GameObject Placement;
    public GameObject GuideScreen;

    bool win = false;

    public GameObject[] prefabs;
    //public List<GameObject> spawned = new List<GameObject>(){};
    
    // 1 = scanned, 0 = not scanned
    // so the user could scan ones that arent the current target and the progress is still tracked, basically
    private List<int> progress = new List<int>(){0,0,0,0,0,0,0,0,0,0};
    private List<int> scanned = new List<int>(){0,0,0,0,0,0,0,0,0,0};

    // text for debugging purposes as we can't use unity debugger pien
    public TextMeshProUGUI debugText;

    public GameObject curPrefab;

    public ARRaycastManager raycastManager;

    private int scannedObjectIndex;

    public Camera ARCamera;

    int guideProgress = 0;

    // Start is called before the first frame update
    void Start()
    {
        // listen to the event for images being scanned
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
        else
        {
            Debug.LogError("ARTrackedImageManager is not assigned!");
        }

        //roomText.text = "INB 1301 - 1st Floor";
        //posterImage.sprite = posterImages[0];
        NextTarget();

        //trackedImageManager.trackablePrefab = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there's at least one touch input on the screen.
        if (Input.touchCount > 0)
        {
            // Get the first touch event (useful if we're only using single-touch interactions).
            Touch touch = Input.GetTouch(0);
            // Check if the touch is on a UI element
            /*if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return; // Ignore the touch if it's on a UI element
            }*/
            // Check if the touch phase just began (indicating the player has just touched the screen).
            if (touch.phase == TouchPhase.Began)
            {
                // Create a ray from the camera through the touch position on the screen.
                //Ray ray = Camera.main.ScreenPointToRay(touch.position);
                Ray ray = ARCamera.ScreenPointToRay(touch.position);
                //Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 2.0f);
                RaycastHit hit;

                // Perform a raycast to check if the touch intersects with any objects in the AR scene.
                if (Physics.Raycast(ray, out hit))
                {
                    //debugText.text = "Hit object: " + hit.collider.gameObject.name;

                    // Get the GameObject that was hit by the raycast.
                    GameObject touchedObject = hit.collider.gameObject;

                    //string name = touchedObject.name.Replace("(Clone)", "");

                    // get the index of the gameobject touched (so we can do the correct popup)
                    //scannedObjectIndex = Array.IndexOf(prefabs, name);
                    for (int i = 0; i < prefabs.Length; i++)
                    {
                        if(touchedObject.name.Replace("(Clone)", "") == prefabs[i].name)
                        {
                            scannedObjectIndex = i;
                            break;
                        }
                    }

                    if(progress[scannedObjectIndex] == 0)
                    {
                        bodyText.text = bodyContent[scannedObjectIndex] + "";
                        titleText.text = charaNames[scannedObjectIndex] + "'s favourite module";
                        objectImage.sprite = images[scannedObjectIndex];

                        // deactivate relevant screens for now
                        InfoScreen.SetActive(true);
                        MainScreenMenu.SetActive(false);
                        ARSystem.SetActive(false);

                        // set variable so we know that this has been scanned already
                        scanned[scannedObjectIndex] = 1;
                        progress[scannedObjectIndex] = 1;

                        NextTarget();
                    }

                    Destroy(touchedObject, 0.0f);

                    //debugText.text = "Object: " + touchedObject.name;
                }

            }
        }
    }

    // called every time the user scans an image
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs events)
    {
        foreach (var trackedImage in events.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                // get the name of the image scanned so we can carry out the relevant events
                string scannedImageName = trackedImage.referenceImage.name;
                OnImageScanned(scannedImageName, trackedImage);
            }
        }
    }

    private void OnImageScanned(string imageName, ARTrackedImage trackedImage)
    {
        // change the prefab to the correct one
        int index = Array.IndexOf(imageNames, imageName);
        curPrefab = prefabs[index];

        //debugText.text = "Image: " + imageName + "Prefab: " + curPrefab.name;

        //trackedImageManager.trackedImagePrefab = curPrefab;

        // if the image hasnt been scanned yet, spawn the object 
        if(scanned[index] == 0)
        {
            GameObject newPrefab = Instantiate(curPrefab, trackedImage.transform.position, trackedImage.transform.rotation, trackedImage.transform);
        }

        // mark this image as scanned already
        scanned[index] = 1;

    }

    private void NextTarget()
    {
        // find the first image that has not been scanned
        for (int i = 0; i < imageNames.Length; i++)
        {
            if(progress[i] == 0)
            {
                guideProgress = i;
                break;
            }

        }

        // game end criteria
        if (progress.All(value => value == 1))
        {
            win = true;
        }

        // set relevant info on guide screen
        if (guideProgress >= 0 && guideProgress <= 2)
        {
            roomText.text = "INB 1301 - 1st Floor";
        }
        else if (guideProgress >= 3 && guideProgress <= 5) 
        {
            roomText.text = "INB 2102 - 2nd Floor";
        }
        else if (guideProgress >= 6 && guideProgress <= 7)
        {
            roomText.text = "INB 1101 - First Floor";
        }
        else if (guideProgress >=8 && guideProgress <= 9)
        {
            roomText.text = "INB 1102 - First Floor";
        }

        posterImage.sprite = posterImages[guideProgress];

        debugText.text = "Progress: " + guideProgress +"\nProgress value: " + progress[guideProgress];

    }

    void GameEnd()
    {
        WinScreen.SetActive(true);
        GuideButton1.SetActive(false);
        GuideScreen.SetActive(false);
        GuideButton2.SetActive(true);
        Placement.SetActive(true);
    }

public void OnPopupClose()
{
    if (win == true)
    {
        GameEnd();
    }
}

}


