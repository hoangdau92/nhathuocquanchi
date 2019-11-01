var NUMINCART = 0;
var QUANCHI_DonHang = {

    AddtoCart: function (id) {
        let checkdangnhap = $(".hi-user").length;
        if (checkdangnhap == 0) {
            alert("Vui lòng đăng nhập trước khi mua sản phẩm. Xin cảm ơn.");
            return;
        }
        if ($("#khdangnhap").length == 0) {
            alert("Quản trị viên không được mua hàng.");
            return;
        }

        var imgtodrag = $('.img_' + id).eq(0);
        if (imgtodrag.length <= 0) {
            imgtodrag = $('.sanpham_' + id).eq(0);
        }
        QUANCHI_DonHang.AnimationCart(imgtodrag);
        let quantity = $("#sp_" + id).val();
        if (quantity == undefined || !$.isNumeric(quantity) || quantity < 1) {
            quantity = 1;
        }
        var pJson = { "idthuoc": id, "quantity": quantity };
        jQuery.ajax({
            url: SiteUrl + "/DonHang/AddToCart",
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
    },

    AnimationCart: function (imgtodrag) {
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
                QUANCHI_DonHang.ChangeNumCart(NUMINCART);
            });
        }
    },

    ChangeNumCart: function (soluong) {
        $("#cart-count").text(soluong);
    },

    OnchangeQuantity: function () {
        $("input.ip-quantity").change(function () {
            var INP = $(this);
            if (!$.isNumeric(INP.val()) || INP.val() <= 0) {
                INP.val(1);
            }
            else {
                INP.val(parseInt(INP.val()));
            }
            ////////////
            var content = `<a href="javascript:void(0)" onclick="QUANCHI_DonHang.UpdateQuantity(${INP.attr("id")})" class="uquantity">Cập nhật</a>`;
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
    },

    UpdateQuantity: function (idsanpham) {
        var q = $("#" + idsanpham).val();
        var pJson = { "idsanpham": idsanpham, "quantity": q };
        jQuery.ajax({
            url: SiteUrl + "/donhang/UpdateQuantity",
            contentType: 'application/json',
            data: pJson,
            dataType: 'json',
            type: "GET",
            success: function (r) {
                if (r.status == true) {
                    QUANCHI_DonHang.BindData(r.lCart);
                }
                else {
                    alert(r.message);
                }
            }
        });
    },

    BindData: function (data) {
        let content = ``;
        let tong = 0;
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                let thanhtien = data[i].giatien * data[i].soluong;
                tong += thanhtien;
                content += `<tr>`;
                content += `<td><img alt="anh dai dien" src="${SiteUrl}${data[i].anhdaidien}"/></td>`;
                content += `<td>${data[i].tensanpham}</td>`;
                content += `<td align="center">${numberToMoney(data[i].giatien)}</td>`;
                content += `<td align="center"><input type="number"  value="${data[i].soluong}" class="form-control ip-quantity" data-current="${data[i].soluong}" id="${data[i].idsanpham}"/></td>`;
                content += `<td align="center">${data[i].donvitinh}</td>`;
                content += `<td align="center">${numberToMoney(thanhtien)}</td>`;
                content += `<td align="center"><a href="javascript:void(0)" onclick="QUANCHI_DonHang.RemoveProduct(${data[i].idsanpham})" title="Xóa khỏi giỏ hàng"><i class="fa fa-times fa-2x text-danger" aria-hidden="true"></i></a></td>`;
                content += `</tr>`;
            }
            //ma giam gia
            content += `<tr>`;
            content += `<td></td>`;
            content += `<td align="right"><b>Mã giảm giá:</b></td>`;
            content += `<td align="center" colspan="2">`;
            content += `<input class="form-control ip-magiamgia" type="text" id="qc-magiamgia"/>`;
            content += `<span class="text-danger" id="mgg-error"></span>`;
            content += `</td>`;
            content += `<td align="center"><a class="btn-magiamgia" href="javascript:void(0)" onclick="QUANCHI_DonHang.Apply()" title="Áp dụng mã giảm giá">Áp dụng</a></td>`;
            content += `<td align="center"><b class="text-danger" id="tiengiam"></b></td>`;
            content += `<td align="center"><i class="fa fa-spinner fa-spin fa-2x fa-fw qc-loading hidden"></i><span class="sr-only qc-loading hidden">Loading...</span></td>`;
            content += `</tr>`;
            ////
            content += `<tr>`;
            content += `<td colspan="4"></td><td><b>Thành tiền:</b></td>`;
            content += `<td align="center">`;
            content += `<b id="thanhtien">${numberToMoney(tong)}</b>`;
            content += `<input type="hidden" value="${tong}" id="tongthanhtien"/>`;
            content += `</td>`;
            content += `<td></td>`;
            content += `</tr>`;
        }
        else {
            content += `<tr><td colspan="100%">Chưa có sản phẩm nào.</td></tr>`;
        }
        $("#giohang").html(content);
        QUANCHI_DonHang.OnchangeQuantity();
        replaceUrl();
    },

    SetMaGiamGia: function () {
        $('.qc-loading').removeClass("hidden");
        $("mgg-error").text("");
        let magiamgia = $("#qc-magiamgia").val();
        var pJson = { "magiamgia": magiamgia };
        jQuery.ajax({
            url: SiteUrl + "/DonHang/ApDungMaGiamGia",
            contentType: 'application/json',
            data: pJson,
            dataType: 'json',
            type: "GET",
            success: function (r) {
                if (r.status == true) {
                    if (r.giatrigiam != 0) {
                        $("#tiengiam").text("-" + numberToMoney(r.giatrigiam));
                        let thanhtien = $('#tongthanhtien').val() - r.giatrigiam;
                        $("#hid-sotiengiam").val(r.giatrigiam);
                        $("#hid-mgg").val(magiamgia);
                        $("#thanhtien").text(numberToMoney(thanhtien));
                    }
                }
                else {
                    $("#mgg-error").text(r.message);
                }
                $('.qc-loading').addClass("hidden");
            }
        });
    },

    RemoveProduct: function (idsanpham) {
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
                            url: SiteUrl + "/donhang/RemoveProduct",
                            contentType: 'application/json',
                            data: pJson,
                            dataType: 'json',
                            type: "GET",
                            success: function (r) {
                                if (r.status == true) {
                                    QUANCHI_DonHang.BindData(r.lCart);
                                    QUANCHI_DonHang.ChangeNumCart(r.soluong);
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
    },

    GetCart: function (magiamgia) {
        jQuery.ajax({
            url: SiteUrl + "/donhang/Getcart",
            contentType: 'application/json',
            dataType: 'json',
            type: "GET",
            success: function (r) {
                if (r.status == true) {
                    let data = r.lCart;
                    QUANCHI_DonHang.BindData(r.lCart);
                    if (magiamgia != "") {
                        $("#qc-magiamgia").val(magiamgia);
                        QUANCHI_DonHang.SetMaGiamGia();
                    }
                }
                else {
                    alert(r.message);
                }
            }
        });
    },

    Apply: function () {
        let magiamgia = $("#qc-magiamgia").val();
        QUANCHI_DonHang.GetCart(magiamgia);
    },

    ThanhToan: function () {
        let diachigiaohang = $("#diachigiaohang").val();
        if (diachigiaohang == "") {
            alert("Bạn chưa nhập địa chỉ giao hàng.");
            return;
        }
        $("#btnthanhtoan i").removeClass("hidden");
        setTimeout(function () {
            let mgg = $("#hid-mgg").val();
            let madonhang = $("#madonhang").text();
            let ghichu = $("#ghichu").val();
            let ptthanhtoan = $('input[name=phuongthucthanhtoan]:checked').val();
            let sotiengiam = $("#hid-sotiengiam").val();
            var pJson = { magiamgia: mgg, ghichu: ghichu, ptthanhtoan: ptthanhtoan, sotiengiam: sotiengiam, madonhang: madonhang, diachigiaohang: diachigiaohang };
            jQuery.ajax({
                url: SiteUrl + "/donhang/ThanhToan",
                contentType: 'application/json',
                dataType: 'json',
                data: pJson,
                type: "GET",
                success: function (r) {
                    if (r.status == true) {
                        window.location.replace(SiteUrl + "/don-hang/thanhcong");
                    }
                    else {
                        alert(r.message);
                    }
                    $("#btnthanhtoan i").addClass("hidden");
                }
            });
        }, 200);
    },
}