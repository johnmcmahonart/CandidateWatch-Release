using System;
using System.Collections.Generic;
using System.Text;
using FECIngest.Client;
using FECIngest.Model;
using FECIngest.FECApi;
namespace FECIngest
{
    public class FECPageResultScheduleB : IFECResultPage
    {

        private bool _isLastPage;
        public bool IsLastPage => _isLastPage;

        private int _totalCount;
        public int TotalCount => _totalCount;
        private ScheduleBByRecipientIDPage _page;
        public object PageData => _page;

        private void CheckLastPage()
        {
            if (_page.Pagination.Page < _page.Pagination.Pages)
                _isLastPage = false;
            else
                _isLastPage = true;
        }

        public FECPageResultScheduleB(ScheduleBByRecipientIDPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            
            CheckLastPage();


        }
    }
}
