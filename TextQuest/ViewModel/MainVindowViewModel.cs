using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextQuest.Classes;
using AngleSharp;
using AngleSharp.Html.Dom;

namespace TextQuest.ViewModel
{
    class MainVindowViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<Quest> Quests { get; set; }
        public Dictionary<String, String> Hashs;



        public static CookieContainer Cookie = new CookieContainer();
        public static HttpClient Client = new HttpClient();
        public static String runtype;
        public static String hash;
        public static String text;
        public static List<String> locations;
        public static String location;
        public static String text_socket;


        private String textQuest;
        public String TextQuest
        {
            get { return textQuest; }
            set
            {
                textQuest = value;
                OnPropertyChanged();
            }
        }


        public static Quest SelectedQuest { get; set; }
        public bool CanStartGame { get; set; }


        public MainVindowViewModel()
        {
            Quests = new ObservableCollection<Quest>();
            locations = new List<string>();
            CanStartGame = false;
            TextQuest = "Текст";
            VisibleFalseBtns();
            CanSearch = true;
        }


        #region props

        private RelayCommand loadQuests;
        public RelayCommand LoadQuests => loadQuests ?? (loadQuests = new RelayCommand(obj =>
        {
            GetQuests();
            CanSearch = false;
        }));

        private Quest selectQuest;
        public Quest SelectQuest
        {
            get 
            {
                if (selectQuest!=null)
                {
                    TextQuest = selectQuest.Description;
                }
                return selectQuest;
            }
            set
            {
                selectQuest = value;
                SelectedQuest = value;
                CanStart = true;
                OnPropertyChanged("SelectedQuest");
            }
        }

        private bool canStart;
        public bool CanStart
        {
            get { return canStart; }
            set
            {
                canStart = value;
                CanStartGame = value;
                OnPropertyChanged();
            }
        }

        private bool canSearch;
        public bool CanSearch
        {
            get { return canSearch; }
            set
            {
                canSearch = value;
                OnPropertyChanged();
            }
        }



        private RelayCommand answer1;
        public RelayCommand Answer1 => answer1 ?? (answer1 = new RelayCommand(obj =>
        {
            SelectAnswer(1);
        }));
        private RelayCommand answer2;
        public RelayCommand Answer2 => answer2 ?? (answer2 = new RelayCommand(obj =>
        {
            SelectAnswer(2);
        }));
        private RelayCommand answer3;
        public RelayCommand Answer3 => answer3 ?? (answer3 = new RelayCommand(obj =>
        {
            SelectAnswer(3);
        }));
        private RelayCommand answer4;
        public RelayCommand Answer4 => answer4 ?? (answer4 = new RelayCommand(obj =>
        {
            SelectAnswer(4);
        }));


        private String btnText1;
        public String BtnText1
        {
            get { return btnText1; }
            set
            {
                btnText1 = value;
                OnPropertyChanged();
            }
        }

        private String btnText2;
        public String BtnText2
        {
            get { return btnText2; }
            set
            {
                btnText2 = value;
                OnPropertyChanged();
            }
        }

        private String btnText3;
        public String BtnText3
        {
            get { return btnText3; }
            set
            {
                btnText3 = value;
                OnPropertyChanged();
            }
        }

        private String btnText4;
        public String BtnText4
        {
            get { return btnText4; }
            set
            {
                btnText4 = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand startGame;
        public RelayCommand StartGame => startGame ?? (startGame = new RelayCommand(obj =>
        {

            runtype = "";
            hash = "";
            locations.Clear();
            location = "";

            Clear();
            ClearBtnText();
            VisibleFalseBtns();
            Step();
            LoadImage();
        }));


        private bool btnVisible1;
        public bool BtnVisible1
        {
            get { return btnVisible1; }
            set
            {
                btnVisible1 = value;
                OnPropertyChanged();
            }
        }

        private bool btnVisible2;
        public bool BtnVisible2
        {
            get { return btnVisible2; }
            set
            {
                btnVisible2 = value;
                OnPropertyChanged();
            }
        }

        private bool btnVisible3;
        public bool BtnVisible3
        {
            get { return btnVisible3; }
            set
            {
                btnVisible3 = value;
                OnPropertyChanged();
            }
        }

        private bool btnVisible4;
        public bool BtnVisible4
        {
            get { return btnVisible4; }
            set
            {
                btnVisible4 = value;
                OnPropertyChanged();
            }
        }

        private String linkImg;
        public String LinkImg
        {
            get { return linkImg; }
            set
            {
                linkImg = value;
                OnPropertyChanged();
            }
        }


        #endregion












        private void Clear()
        {
            TextQuest = "";
            ClearBtnText();
        }
        private void ClearBtnText()
        {
            BtnText1 = "";
            BtnText2 = "";
            BtnText3 = "";
            BtnText4 = "";
        }
        private void VisibleFalseBtns()
        {
            BtnVisible1 = false;
            BtnVisible2 = false;
            BtnVisible3 = false;
            BtnVisible4 = false;
        }
        private void LoadImage()
        {
            Task.Run(async () =>
            {
                var req = await Client.GetAsync(SelectQuest.Link);
                var res = req.Content.ReadAsStringAsync().Result;
                LinkImg = Regex.Match(res, "<img src=\"(https://apero.ru/public/img/previews/.*?png)").Groups[1].Value;

            });
        }


        private void SelectAnswer(int numberBtn)
        {
            int i = 1;
            KeyValuePair<String,String> item = new KeyValuePair<string, string>();
            foreach(var hash in Hashs)
            {
                if (i==numberBtn)
                {
                    item = hash;
                    location = locations[i-1];
                    break;
                }
                i++;
            }

            Task.Run(async () =>
            {
                await Player(item.Key);
            });

        }





        public void GetQuests()
        {
            Task.Run(() =>
            {
                //var html = Utils.OpenLink("https://apero.ru/%D0%A2%D0%B5%D0%BA%D1%81%D1%82%D0%BE%D0%B2%D1%8B%D0%B5-%D0%B8%D0%B3%D1%80%D1%8B");
                for (int page=1;page<10;page++)
                {
                    var html = Utils.OpenLink("https://apero.ru/%D0%A2%D0%B5%D0%BA%D1%81%D1%82%D0%BE%D0%B2%D1%8B%D0%B5-%D0%B8%D0%B3%D1%80%D1%8B/%D0%9A%D0%B0%D1%82%D0%B0%D0%BB%D0%BE%D0%B3/"+page);

                    var tagQuests = Utils.Doc.QuerySelectorAll("div[class='tabled-game-block']");

                    for (int i = 0; i < tagQuests.Count(); i++)
                    {
                        Quest quest = new Quest();
                        quest.Title = tagQuests[i].Children[1].Children[1].TextContent;
                        quest.Description = tagQuests[i].Children[1].Children[6].TextContent;
                        quest.Link = tagQuests[i].Children[0].Children[0].GetAttribute("Href");
                        AddQuest(quest);
                    }
                }
                
            });
            
        }


        private void AddQuest(Quest quest)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Quests.Add(quest);
            });
            OnPropertyChanged("Quests");
        }



        public async void Step()
        {
            await Player();
            await Socket();
        }

        public async Task<String> Player(String hash = null)
        {

            if (location==null)
            {
                location = "";
            }

            String run = "";
            if (runtype != null)
            {
                run = runtype;
            }

            var dict = new Dictionary<String, String>
                {
                    { "location",location},
                    { "gameUrl",SelectedQuest.Link.Split('/')[SelectedQuest.Link.Split('/').Count()-1]},
                    { "runtype",run==""?"new":run},
                    { "inputcnt","0"},
                };

            if (hash != null)
            {
                if (hash!="") dict.Add("hash", hash);
            }
            
            var request = await Client.PostAsync("https://apero.ru/player", new FormUrlEncodedContent(dict));
            var lines = Regex.Matches(Regex.Unescape(request.Content.ReadAsStringAsync().Result), "<p >(.*?)</p>");
            if (runtype == null)
            {
                runtype = Regex.Match(Regex.Unescape(request.Content.ReadAsStringAsync().Result), "game_session_id\":(\\d+)}").Groups[1].Value;
            }
            
            var locat = Regex.Matches(Regex.Unescape(request.Content.ReadAsStringAsync().Result), "data-location= \"(.*?)\"");
            locations.Clear();
            for (int i=0;i<locat.Count;i++)
            {
                locations.Add(locat[i].Groups[1].Value);
            }

            if (lines!=null)
            {
                if(lines.Count>0)
                {
                    var allText = "";
                    for(int i=0;i<lines.Count;i++)
                    {
                        if (lines[i].Groups[1].Value.Contains("span")) continue;
                        allText += lines[i].Groups[1].Value + Environment.NewLine;
                    }
                    TextQuest = allText;
                }
            }

            var btns = Regex.Matches(Regex.Unescape(request.Content.ReadAsStringAsync().Result), "data-hash=\"(.*?)\".*?data-location.*?\" >(.*?)<");
            setTextBtn(btns);

            return Regex.Unescape(request.Content.ReadAsStringAsync().Result);
        }


        private void setTextBtn(MatchCollection btnsText)
        {
            ClearBtnText();
            VisibleFalseBtns();
            Hashs = new Dictionary<string, string>();
            for (int i=0;i<btnsText.Count;i++)
            {
                switch(i)
                {
                    case 0:
                        BtnText1 = btnsText[i].Groups[2].Value;
                        BtnVisible1 = true;
                        break;
                    case 1:
                        BtnText2 = btnsText[i].Groups[2].Value;
                        BtnVisible2 = true;
                        break;
                    case 2:
                        BtnText3 = btnsText[i].Groups[2].Value;
                        BtnVisible3 = true;
                        break;
                    case 3:
                        BtnText4 = btnsText[i].Groups[2].Value;
                        BtnVisible4 = true;
                        break;
                }
                Hashs.Add(btnsText[i].Groups[1].Value, btnsText[i].Groups[2].Value);
            }
        }



        public async Task<String> Socket()
        {
            var dict = new Dictionary<String, String>
                {
                    { "gameUrl","История-Державы"},
                    { "sec","300"},
                    { "runtype",runtype},
                };
            var request = await Client.PostAsync("https://apero.ru/my_socket", new FormUrlEncodedContent(dict));
            return Regex.Unescape(request.Content.ReadAsStringAsync().Result);
        }




        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }





}
