using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    // 싱글톤
    public static PuzzleManager instance;

    // 퍼즐의 갯수
    const int PUZZLECOUNT = 30;

    // 퍼즐 리스트
    public bool[] puzzles;

    // 퍼즐의 클리어 여부를 담은 딕셔너리 key : 퍼즐인덱스, value : 클리어 여부
    public Dictionary<int, bool> puzzleClearDictionary = new Dictionary<int, bool>();

    #region 퍼즐 클리어 스크립트
    // 퍼즐 클리어 스크립트들입니다. [ 순서대로 퍼즐 번호 ] DB에서 퍼즐 클리어의 여부에 따라 해당 퍼즐 스크립트에 클리어 여부를 체크해주기 위해 캐싱해두었음.
    [field: SerializeField] public Upon3treePuzzleClear upon3TreePuzzleClear { get; private set; }      // 0
    [field: SerializeField] public LetterPuzzleClear letterPuzzleClear { get; private set; }            // 1
    [field: SerializeField] public FlowerPuzzleClear flowerPuzzleClear { get; private set; }            // 2
    [field: SerializeField] public ChessPuzzleClear chessPuzzleClear { get; private set; }              // 3
    [field: SerializeField] public BookSortPuzzleClear bookSortPuzzleClear { get; private set; }        // 4
    [field: SerializeField] public TreePuzzleClear treePuzzleClear { get; private set; }                // 5
    [field: SerializeField] public ShieldPuzzleClear shieldPuzzleClear { get; private set; }            // 6
    [field: SerializeField] public StudioPuzzleClear studioPuzzleClear { get; private set; }            // 7
    [field: SerializeField] public TaxidermyClear taxidermyClear { get; private set; }                  // 8
    [field: SerializeField] public AppleCheck appleClear { get; private set; }                          // 9
    [field: SerializeField] public FabricCheck fabricClear { get; private set; }                        // 10

    [field: SerializeField] public PotPuzzleClear potPuzzleClear { get; private set; }                  // 11
    [field: SerializeField] public ButcherShop01Clear butcherShop01Clear { get; private set; }          // 12
    // TODO : 듀토리얼 퍼즐 01 없어짐                                                                     // 13
    [field: SerializeField] public Tutorial01Clear tutorial02Clear { get; private set; }                // 14
    [field: SerializeField] public ButcherShop02Clear butcherShop02Clear { get; private set; }          // 15
    [field: SerializeField] public ButcherShop03Clear butcherShop03Clear { get; private set; }          // 16
    [field: SerializeField] public HiddenPuzzleClear hiddenPuzzleClear { get; private set; }            // 17
    [field: SerializeField] public FlowerDelivery flowerDeliveryPuzzleClear { get; private set; }       // 18
    [field: SerializeField] public BulbChangeClear bulbChangeClear { get; private set; }                // 19
    [field: SerializeField] public PerfumePuzzleClear perfumePuzzleClear { get; private set; }          // 20
    [field: SerializeField] public HideAndSeek hideAndSeekPuzzleClear { get; private set; }             // 21
    [field: SerializeField] public CatPuzzleClear catPuzzleClear { get; private set; }                  // 22
                                                                                                        // 개발할 퍼즐 목표 종료.
    #endregion

    #region 퍼즐 벽 스크립트
    [Space(10f)]
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Upon3tree_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Letter_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_FlowerColor_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Chess_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Book_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_DropTree_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Shield_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Studio_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_taxidermy_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_UponApple_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Fabric_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Pot_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_ButcherShop01_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Tutorial02Clear_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_ButcherShop02_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_ButcherShop03_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Hidden_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_flowerDelivery_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_BulbChange_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Perfume_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_HideSeek_Wall;
    [field: SerializeField] public PlayerEnterPuzzleTrigger Puzzle_Cat_Wall;
    #endregion

    // 파이어 베이스
    private DatabaseReference reference;    // 루트 레퍼런스

    private void Awake()
    {
        // { 싱글톤
        if (null == instance)
        {
            instance = this;

            // LEGACY : 메인 씬에서만 PuzzleManager를 사용할 예정
            // DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }       // } 싱글톤

        // { 퍼즐 클리어 초기화
        puzzles = new bool[PUZZLECOUNT];

        for (int i = 0; i < PUZZLECOUNT; i++)
        {
            puzzles[i] = false;
            CheckPuzzleClear(i, puzzles[i]);
        }       // } 퍼즐 클리어 초기화
    }

    private void Start()
    {
        // 퍼즐 DB 받아오기
        FirebaseManager.instance.PuzzleClearUpdateFromDB();

        // 클리어 별의 갯수 초기화
        StarManager.starManager.InitStar();
    }

    /// <summary>
    /// 퍼즐의 클리어 여부를 판단해서 퍼즐클리어 배열과 딕셔너리를 업데이트하는 함수
    /// </summary>
    /// <param name="_puzzleIndex">퍼즐인덱스</param>
    /// <param name="_isClear">클리어여부</param>
    public void CheckPuzzleClear(int _puzzleIndex, bool _isClear)
    {
        // 배열 업데이트
        puzzles[_puzzleIndex] = _isClear;

        // 딕셔너리 업데이트 [ key가 있다면 value 업데이트 ]
        if (puzzleClearDictionary.ContainsKey(_puzzleIndex))
        {
            puzzleClearDictionary[_puzzleIndex] = _isClear;
        }

        else
        {
            puzzleClearDictionary.Add(_puzzleIndex, _isClear);
        }
    }

    /// <summary>
    /// 클리어 여부를 체크해서 펫말과 퍼즐벽을 활성 / 비활성화 하는 메서드
    /// </summary>
    public void ActiveSign()
    {
        // 퍼즐이 클리어가 되었다면 팻말 세우기
        // 퍼즐 확정 되고 리팩토링 할 시간이 주어진다면 퀘스트 목록, 퀘스트 투명 벽 리스트 혹은 배열로 선언해서 코드 깔끔하게 하고싶음.
        for (int i = 0; i < PUZZLECOUNT; i++)
        {
            if (i == 0) { upon3TreePuzzleClear.ActiveClearSign(puzzles[i]);         Puzzle_Upon3tree_Wall.isPuzzleClear = puzzles[i];       }
            else if (i == 1) { letterPuzzleClear.ActiveClearSign(puzzles[i]);       Puzzle_Letter_Wall.isPuzzleClear = puzzles[i];          }
            else if (i == 2) { flowerPuzzleClear.ActiveClearSign(puzzles[i]);       Puzzle_FlowerColor_Wall.isPuzzleClear = puzzles[i];     }
            else if (i == 3) { chessPuzzleClear.ActiveClearSign(puzzles[i]);        Puzzle_Chess_Wall.isPuzzleClear = puzzles[i];           }
            else if (i == 4) { bookSortPuzzleClear.ActiveClearSign(puzzles[i]);     Puzzle_Book_Wall.isPuzzleClear = puzzles[i];            }
            else if (i == 5) { treePuzzleClear.ActiveClearSign(puzzles[i]);         Puzzle_DropTree_Wall.isPuzzleClear = puzzles[i];        }
            else if (i == 6) { shieldPuzzleClear.ActiveClearSign(puzzles[i]);       Puzzle_Shield_Wall.isPuzzleClear = puzzles[i];          }
            else if (i == 7) { studioPuzzleClear.ActiveClearSign(puzzles[i]);       Puzzle_Studio_Wall.isPuzzleClear = puzzles[i];          }
            else if (i == 8) { taxidermyClear.ActiveClearSign(puzzles[i]);          Puzzle_taxidermy_Wall.isPuzzleClear = puzzles[i];       }
            else if (i == 9) { appleClear.ActiveClearSign(puzzles[i]);              Puzzle_UponApple_Wall.isPuzzleClear = puzzles[i];       }
            else if (i == 10) { fabricClear.ActiveClearSign(puzzles[i]);            Puzzle_Fabric_Wall.isPuzzleClear = puzzles[i];          }    
            else if (i == 11) { potPuzzleClear.ActiveClearSign(puzzles[i]);         Puzzle_Pot_Wall.isPuzzleClear = puzzles[i];             }
            else if (i == 12) { butcherShop01Clear.ActiveClearSign(puzzles[i]);     Puzzle_ButcherShop01_Wall.isPuzzleClear = puzzles[i];   }
            /* else if(i == 13) { TODO : 듀토리얼 퍼즐 01 } */
            else if (i == 14) { tutorial02Clear.ActiveClearSign(puzzles[i]);        Puzzle_Tutorial02Clear_Wall.isPuzzleClear = puzzles[i]; }
            else if (i == 15) { butcherShop02Clear.ActiveClearSign(puzzles[i]);     Puzzle_ButcherShop02_Wall.isPuzzleClear = puzzles[i];   }
            else if (i == 16) { butcherShop03Clear.ActiveClearSign(puzzles[i]);     Puzzle_ButcherShop03_Wall.isPuzzleClear = puzzles[i];   }
            else if (i == 17) { hiddenPuzzleClear.ActiveClearSign(puzzles[i]);      Puzzle_Hidden_Wall.isPuzzleClear = puzzles[i];          }
            else if (i == 18) { flowerDeliveryPuzzleClear.ActiveClearSign(puzzles[i]); Puzzle_flowerDelivery_Wall.isPuzzleClear = puzzles[i]; }
            else if (i == 19) { bulbChangeClear.ActiveClearSign(puzzles[i]);        Puzzle_BulbChange_Wall.isPuzzleClear = puzzles[i];      }
            else if (i == 20) { perfumePuzzleClear.ActiveClearSign(puzzles[i]);     Puzzle_Perfume_Wall.isPuzzleClear = puzzles[i];         }
            else if (i == 21) { hideAndSeekPuzzleClear.ActiveClearSign(puzzles[i]); Puzzle_HideSeek_Wall.isPuzzleClear = puzzles[i];        }
            else if (i == 22) { catPuzzleClear.ActiveClearSign(puzzles[i]);         Puzzle_Cat_Wall.isPuzzleClear = puzzles[i];             }
        }
    }

    /// <summary>
    /// 획득한 총 별의 갯수를 설정해줍니다.
    /// </summary>
    public void InitAllStarCount()
    {
        int starCount = 0;

        foreach (bool _isClear in puzzles)
        {
            if (_isClear)
            {
                starCount++;
            }
        }

        // 획득한 총 별의 수 셋팅
        StarManager.starManager.getStarCount = starCount;
    }
}
