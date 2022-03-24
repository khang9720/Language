var homeconfig = {
    pageSize: 10,
    pageIndex: 1,
}
var valueB;
var languageController = {
    init: function () {
        languageController.loadData();
        languageController.registerEvent();
    },
    registerEvent: function () {
        $('.btn-delete').off('click').on('click', function () {
            var id = $(this).data('id');
            bootbox.confirm({
                message: "Bạn có chắc muốn xóa?",
                buttons: {
                    confirm: {
                        lable: "Có",
                        className: 'btn-success'
                    },
                    cancel: {
                        label: 'Không',
                        className: 'btn-danger'
                    }
                },
                callback: function (result) {
                    if (result) {
                        languageController.deleteList(id);
                        console.log('This was logged in the callback!');
                    }
                    
                }
            })
        });
        $('.btn-cancel').off('click').on('click', function () {
            languageController.loadData();
        });

        //$('.btn-edit').off('click').on('click', function () {
        //    document.getElementById("btn-save").disabled = false;
            
        //    var id = $(this).data('id');
        //    languageController.loadDetail(id);
        //    $('#modalAddUpdate').modal('show');
        //});

        $('.btn-edit').off('click').on('click', function () {
            var key = $(this).data('id');
            var sib = "td." + key;
            var value;
            var index = 0; 
            $(this)
                .parent()
                .siblings(sib)
                .each(function () {
                    var content = $(this).html();
                    if (index == 0) {
                        valueB = content
                    }
                    $(this).html('<input  id="' + key + '" value="' + content + '" />');

                    index++;
                });
            $(this).siblings(".btn-cancel").show();
            $(this).siblings(".btn-save").show();
            $(this).siblings(".btn-delete").hide();
            $(this).hide();
        });

        $('.btn-save').off('click').on('click', function () {
            var index = 0;
            var value;
            $("input").each(function () {
                var content = $(this).val();
                if (index == 0) {
                    value = content;
                }
                $(this).html(content);
                $(this).contents().unwrap();
                index++;
            });
            var key = $(this).data('id');

            if (valueB == value) {
                bootbox.alert("Thông tin chưa được thay đổi?")
            }
            else if (value == "") {
                bootbox.alert("Value không được chống")
            }
            else {
                
                languageController.loadEdit(key, value);
                $(this).siblings(".btn-edit").show();
                $(this).siblings(".btn-delete").show();
                $(this).siblings(".btn-cancel").hide();
                $(this).hide();
                bootbox.alert("Thay đổi thông tin thành công", function () {
                    languageController.loadData();
                });
            }
        });
    },
    //loadDetail: function (key) {
    //    $.ajax({
    //        url: '/language/Edit',
    //        data: {
    //            key: key
    //        },
    //        type: 'GET',
    //        dataType: 'json',
    //        success: function (response) {
    //            if (response.status == true) {
    //                var key = "#"+response.key;
    //                var data = response.data;
    //                $('#txtKey').val(data.key);
    //                $('#txtValue').val(data.value);
    //                $('#ckStatus').prop('checked', data.Status);
    //            }
    //            else {
    //                bootbox.alert("XXXX");
    //            }
    //        },
    //        error: function (err) {
    //            console.log(err);
    //        }
    //    });
    //},
    loadEdit: function (key, value) {
        $.post("/language/Edit", { key: key, valueNew: value });
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
                    var data2 = response.data2;
                    var html = '';
                    var template = $('#data-template').html();
                    $.each(data, function (i, item) {
                        html += Mustache.render(template, {
                            index: i + 1,
                            Del: "@Html.ActionLink('Delete', 'Language', new {key = " + item.key + " })",
                            key: item.key,
                            value: item.value,
                            value2: data2[i]["value"],
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
        });

    },

    deleteList: function (key) {
        $.ajax({
            url: '/language/Delete',
            data: {
                key: key
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status == true) {
                    bootbox.alert("Xóa thành công", function () {
                        languageController.loadData();
                    });
                }
                else {
                    bootbox.alert(response.message);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
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
    },

}
languageController.init()