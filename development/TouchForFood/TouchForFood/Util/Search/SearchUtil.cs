using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.ViewModels;
using TouchForFood.Mappers;
using TouchForFood.Models;
using System.Text.RegularExpressions;

namespace TouchForFood.Util.Search
{
    public class SearchUtil
    {
        // holds menu items - has a 1-to-1 relationship with menu items UNIQUE to a menu
        private IDictionary<int, SearchViewModel> dictionary;

        // holds all words (tokens) in an inverted index with a postings list
        private IDictionary<string, IDictionary<int, IList<int>>> invertedIndex
            = new Dictionary<string, IDictionary<int, IList<int>>>();

        // holds all the dictionaries for all the restaurants - key is resto id
        private IDictionary<int, IDictionary<int, SearchViewModel>> dictionaryHolder
            = new Dictionary<int, IDictionary<int, SearchViewModel>>();

        // holds all the inverted indexes for all the restaurants - key is resto id
        private IDictionary<int, IDictionary<string, IDictionary<int, IList<int>>>> indexHolder
            = new Dictionary<int, IDictionary<string, IDictionary<int, IList<int>>>>();

        // constants for positions in inverted index
        private const int constNameDesc = 0;
        private const int constMetadata = 1;
        private const int constCatName = 2;
        private const int constMenuName = 3;

        public SearchUtil() {}

        /// <summary>
        /// Clears all dictionaries and indexes and loads them with current data
        /// </summary>
        public void ClearAndFill()
        {
            ResetSearch();

            // get a list of valid restaurants
            ICollection<restaurant> restos = new RestaurantIM().find(false);

            foreach (var resto in restos)
            {
                // clear the index
                invertedIndex = new Dictionary<string, IDictionary<int, IList<int>>>();

                // load the dictionary (any previous data will be overwritten)
                dictionary = LoadDictionary(resto.id);

                // fill class variable invertedIndex
                CreateInvertedIndex();

                dictionaryHolder.Add(resto.id, dictionary);
                indexHolder.Add(resto.id, invertedIndex);
            }
            
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["dictionary"] = dictionaryHolder;
            HttpContext.Current.Application["invertedIndex"] = indexHolder;
            HttpContext.Current.Application.UnLock();
        }


        /// <summary>
        /// Clears the dictionary and inverted index from the application memory
        /// </summary>
        private void ResetSearch()
        {
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["dictionary"] = null;
            HttpContext.Current.Application["invertedIndex"] = null;
            HttpContext.Current.Application.UnLock();
        }


        /// <summary>
        /// Loads the dictionary from the db
        /// </summary>
        /// <param name="restoId"></param>
        /// <returns></returns>
        private IDictionary<int, SearchViewModel> LoadDictionary(int restoId)
        {
            var dictionaryList = SearchViewModelHelper.PopulateSearchViewModelList(SearchService.GetAll(restoId).ToList());
            IDictionary<int, SearchViewModel> tempDictionary = new Dictionary<int, SearchViewModel>();

            foreach (var item in dictionaryList)
            {
                tempDictionary.Add(item.menuItemId, item);
            }

            return tempDictionary;
        }



        /// <summary>
        /// Creates the inverted index from the dictionary
        /// </summary>
        private void CreateInvertedIndex()
        {
            // normalize each attribute if it's not null
            // then add it to the inverted index
            foreach (var item in dictionary)
            {
                if (item.Value.name != null)
                {
                    IList<string> tempNameList = Normalize(item.Value.name);
                    PopulateInvertedIndex(tempNameList, item.Value.menuItemId, constNameDesc);
                }

                if (item.Value.description != null)
                {
                    IList<string> tempDescList = Normalize(item.Value.description);
                    PopulateInvertedIndex(tempDescList, item.Value.menuItemId, constNameDesc);
                }

                if (item.Value.metadata != null)
                {
                    IList<string> tempMetadataList = Normalize(item.Value.metadata);
                    PopulateInvertedIndex(tempMetadataList, item.Value.menuItemId, constMetadata);
                }

                if (item.Value.catName != null)
                {
                    IList<string> tempCatList = Normalize(item.Value.catName);
                    PopulateInvertedIndex(tempCatList, item.Value.menuItemId, constCatName);
                }

                if (item.Value.menuName != null)
                {
                    IList<string> tempMenuList = Normalize(item.Value.menuName);
                    PopulateInvertedIndex(tempMenuList, item.Value.menuItemId, constMenuName);
                }
            }

        }


        /// <summary>
        /// Fills the inverted index
        /// </summary>
        /// <param name="stringList"></param>
        /// <param name="itemId"></param>
        /// <param name="position"></param>
        private void PopulateInvertedIndex(IList<string> stringList, int itemId, int position)
        {
            foreach (var item in stringList)
            {
                if (invertedIndex.ContainsKey(item))
                {
                    if (invertedIndex[item].ContainsKey(itemId))
                    {
                        invertedIndex[item][itemId][position] = invertedIndex[item][itemId][position] + 1;
                    }
                    else
                    {
                        IList<int> tempList = new List<int> { 0, 0, 0, 0 };
                        tempList[position] = tempList[position] + 1;
                        invertedIndex[item].Add(itemId, tempList);
                    }
                }
                else
                {
                    IList<int> tempList = new List<int> { 0, 0, 0, 0 };
                    tempList[position] = tempList[position] + 1;
                    IDictionary<int, IList<int>> tempPostings = new Dictionary<int, IList<int>>();
                    tempPostings.Add(itemId, tempList);
                    invertedIndex.Add(item, tempPostings);
                }
            }
        }


        /// <summary>
        /// Does string normalization on the search string
        /// - to lowercase
        /// - remove whitespace
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public static IList<string> Normalize(string searchString)
        {
            // split the search string and remove empty strings
            string[] searchArray = searchString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            IList<string> searchList = searchArray.ToList<string>();

            // convert each token to lowercase
            for (int i = 0; i < searchList.Count(); i++)
            {
                string token = searchList.ElementAt(i).ToLower();
                searchList[i] = token;
            }

            return searchList;
        }


    }
}