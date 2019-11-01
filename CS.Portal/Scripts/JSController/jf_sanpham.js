var NUMINCART = 0;

function onchangeQuantity() {
    $("input.quantity").change(function () {
        var INP = $(this);
        if (!$.isNumeric(INP.val()) || INP.val() <= 0) {
            INP.val(1);
        }
        else {
            INP.val(parseInt(INP.val()));
        }
        ////////////
        var content = `<a href="javascript:void(0)" onclick="updateQuantity(${INP.attr("id")})" class="uquantity">Cập nhật</a>`;
        var INP_P = INP.parent();
        let current_val = parseInt(INP.data("current"));
        let new_val = parseInt(INP.val());
        if (current_val != new_val) {
            if (INP_P.find(".uquantity").length == 0) {
                INP_P.append(content);
            }
        }
        else {
            INP_P.find(".uquantity").remove();
        }
    });
}

function updateQuantity(idsanpham) {
    var q = $("#" + idsanpham).val();
    var pJson = { "idsanpham": idsanpham, "quantity": q };
    jQuery.ajax({
        url: SiteUrl + "/sanpham/updateQuantity",
        contentType: 'application/json',
        data: pJson,
        dataType: 'json',
        type: "GET",
        success: function (r) {
            if (r.status == true) {
                bindData(r.lCart);
            }
            else {
                alert(r.message);
            }
        }
    });
}

function removeProduct(idsanpham) {
    jQuery.confirm({
        title: '<i class="fa fa-trash" aria-hidden="true"></i> WARNING',
        content: '<i class="fa fa-arrow-right" aria-hidden="true"></i> Bạn muốn bỏ sản phẩm này?',
        type: 'red',
        async: false,
        buttons: {
            confirm: {
                text: 'Đồng ý',
                btnClass: 'btn-red',
                action: function () {
                    var pJson = { 'idsanpham': idsanpham };
                    jQuery.ajax({
                        url: SiteUrl + "/sanpham/removeproduct",
                        contentType: 'application/json',
                        data: pJson,
                        dataType: 'json',
                        type: "GET",
                        success: function (r) {
                            if (r.status == true) {
                                bindData(r.lCart);
                                changeNumCart(r.soluong);
                            }
                            else {
                                alertError(r.message);
                            }
                        }
                    });
                }
            },
            cancel: {
                text: 'Hủy',
                action: function () {
                }
            }
        }
    });
}

function changeNumCart(soluong) {
    $("#ghsoluong").text(soluong);
}

function animationCart(imgtodrag) {
    var cart = $('.icon-shopping_cart');
    if (imgtodrag) {
        var imgclone = imgtodrag.clone()
            .offset({
                top: imgtodrag.offset().top,
                left: imgtodrag.offset().left
            })
            .css({
                'position': 'absolute',
                'font-weight': 'bold',
                'background': '#fff',
                'z-index': '100',
                'height': '100px',
                'width': '100px'
            })
            .appendTo($('body'))
            .animate({
                'top': cart.offset().top + 10,
                'left': cart.offset().left + 10,
                'width': 75,
                'height': 75
            }, 1000, 'easeInOutExpo');



        imgclone.animate({
            'font-size': '0px',
            'height': 0,
            'width': 0
        }, function () {
            $(this).detach();
            changeNumCart(NUMINCART);
        });
    }
}

function addtocart(id) {
    var imgtodrag = $('.img_' + id).eq(0);
    if (imgtodrag.length <= 0) {
        imgtodrag = $('.sanpham_' + id).eq(0);
    }
    //var imgtodrag = $('.sanpham_' + id).eq(0);
    animationCart(imgtodrag);
    let quantity = $("#sp_" + id).val();
    if (quantity == undefined || !$.isNumeric(quantity) || quantity < 1) {
        quantity = 1;
    }
    var pJson = { "idsanpham": id, "quantity": quantity };
    jQuery.ajax({
        url: SiteUrl + "/sanpham/addtocart",
        contentType: 'application/json',
        data: pJson,
        dataType: 'json',
        type: "GET",
        success: function (r) {
            if (r.status == true) {
                NUMINCART = r.soluong;
                //jQuery.confirm({
                //    title: '<i class="fa fa-bullhorn" aria-hidden="true"></i> Thông báo',
                //    content: '<i class="fa fa-arrow-right" aria-hidden="true"></i> ' + r.message,
                //    type: 'blue',
                //    async: false,
                //    buttons: {
                //        confirm: {
                //            text: 'Thanh toán',
                //            btnClass: 'btn-blue',
                //            action: function () {
                //                window.location.replace(SiteUrl + "/gio-hang");
                //            }
                //        },
                //        cancel: {
                //            text: 'Tiếp tục mua hàng',
                //            action: function () {
                //            }
                //        }
                //    }
                //});
            }
            else {
                alert(r.message);
            }
        }
    });
}

function addtocart_home() {
    var imgtodrag = $('.sanpham_' + id).eq(0);
    animationCart(imgtodrag);
    let quantity = $("#sp_" + id).val();
    if (quantity == undefined || !$.isNumeric(quantity) || quantity < 1) {
        quantity = 1;
    }
    var pJson = { "idsanpham": id, "quantity": quantity };
    jQuery.ajax({
        url: SiteUrl + "/sanpham/addtocart",
        contentType: 'application/json',
        data: pJson,
        dataType: 'json',
        type: "GET",
        success: function (r) {
            if (r.status == true) {
                NUMINCART = r.soluong;
            }
            else {
                alert(r.message);
            }
        }
    });
}


function bindData(data) {
    let content = ``;
    let tong = 0;
    if (data.length > 0) {
        for (var i = 0; i < data.length; i++) {
            let thanhtien = data[i].giatien * data[i].soluong;
            tong += thanhtien;
            content += `<tr>`;
            content += `<td align="center">${i + 1}</td>`;
            content += `<td>${data[i].tensanpham}</td>`;
            content += `<td align="center">${data[i].donvitinh}</td>`;
            content += `<td align="right">${numberToMoney(data[i].giatien)}</td>`;
            content += `<td align="right"><input type="number"  value="${data[i].soluong}" class="form-control quantity" data-current="${data[i].soluong}" id="${data[i].idsanpham}"/></td>`;
            content += `<td align="right">${numberToMoney(thanhtien)}</td>`;
            content += `<td align="center"><a href="javascript:void(0)" onclick="removeProduct(${data[i].idsanpham})" title="Xóa khỏi giỏ hàng"><i class="fa fa-times fa-2x text-danger" aria-hidden="true"></i></a></td>`;
            content += `</tr>`;
        }
        content += `<tr>`;
        content += `<td></td><td colspan="4"><b>Tổng:</b></td>`;
        content += `<td align="right"><b>${numberToMoney(tong)}</b></td>`;
        content += `<td></td>`;
        content += `</tr>`;
    }
    else {
        content += `<tr><td colspan="100%">Chưa có sản phẩm nào.</td></tr>`;
    }
    $("#giohang").html(content);
    onchangeQuantity();
}

function getCart() {
    jQuery.ajax({
        url: SiteUrl + "/sanpham/getcart",
        contentType: 'application/json',
        dataType: 'json',
        type: "GET",
        success: function (r) {
            if (r.status == true) {
                let data = r.lCart;
                bindData(data);
            }
            else {
                alert(r.message);
            }
        }
    });
}