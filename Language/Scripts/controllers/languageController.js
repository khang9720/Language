var homeconfig = {
    pageSize: 10,
    pageIndex: 1,
}
var languageController = {
    init: function () {
        
        languageController.loadData();
        languageController.registerEvent();
    },
    registerEvent: function () {
        
    },
    loadData: function () {
        var name = $('#txtNameS').val();
        var status = $('#ddlStatusS').val();

        $.ajax({
            url: '/language/LoadData',
            type: 'GET',
            data: {
                name: name,
                status: status,
                page: homeconfig.pageIndex,
                pageSize: homeconfig.pageSize
            },
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    var data = response.data;
                    var html = '';
                    var template = $('#data-template').html();
                    $.each(data, function (i, item) {
                        html += Mustache.render(template, {
                            index: i+1 ,
                            ID: item.key,
                            name: item.value,
                            Status: item.Status == true ? "<span class=\"label label-success\">Actived</span>" : "<span class=\"label label-danger\">Locked</span>"
                        })
                    });
                    $('#tbData').html(html);
                    languageController.paging(response.total, function () {
                        languageController.loadData();
                    });
                    languageController.registerEvent();
                }
            }
        })
    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / homeconfig.pageSize);

        if ($('pagination').length === 0 || changePageSize === true) {
            $('pagination').empty();
            $('pagination').removeData('twbs-pagination');
        }

        $('#pagination').twbsPagination({
            totalPages: totalPage,
            first: "Đầu",
            next: "Tiếp",
            last: "Cuối",
            prev: "Trước",
            visiblePages: 10,
            onPageClick: function (event, page) {
                homeconfig.pageIndex = page;
                setTimeout(callback, 200);
            }
        });
    }
}
languageController.init()