var QUANCHI_TaiKhoan = {

    OpenPopup: function (iddonhang) {
        $("#qcModal_ChiTietDonHang").modal("show");
        jQuery('#iddonhang').val(iddonhang);
        var pJson = { "iddonhang": iddonhang };
        jQuery.ajax({
            url: SiteUrl + "/taikhoan/LayThongTinDonHang",
            contentType: 'application/json',
            data: pJson,
            dataType: 'json',
            type: "GET",
            success: function (r) {
                if (r.status == true) {
                    $("#madonhang").text(r.don_hang.MaDonHang);
                    $("#ngaydathang").text(jsonUnixToDateTime(r.don_hang.NgayDatHang));
                    $("#phuongthuc_thanhtoan").text(r.don_hang.PhuongThucThanhToan);
                    let content = ``;
                    if (r.data.length > 0) {
                        for (var i = 0; i < r.data.length; i++) {
                            content += `<tr>`
                            content += `<td align="center">` + (i + 1) + `</td>`;
                            content += '<td>' + r.data[i].TenThuoc + '</td>';
                            content += '<td>' + r.data[i].LoaiThuoc + '</td>';
                            content += '<td align="center">' + r.data[i].DonVi + '</td>';
                            content += '<td align="center">' + r.data[i].SoLuong + '</td>';
                            content += `<td align="right">${numberToMoney(r.data[i].GiaTien)}</td>`;
                            content += `<td align="right">${numberToMoney(r.data[i].ThanhTien)}</td>`;
                            content += `</tr>`
                        }
                        content += `<tr>`
                        content += `<td align="right" colspan="5">Mã giảm giá:</td>`;
                        content += `<td align="center">` + r.tenmagiamgia + `</td>`;
                        content += `<td align="center"> - ${numberToMoney(r.thanhtien.sotiengiam)}</td>`;
                        content += `</tr>`
                       
                        content += `<tr>`
                        content += `<td align="right" colspan="6">Tổng tiền:</td>`;
                        content += `<td align="center">${numberToMoney(r.thanhtien.thanhtiensaugiam)}</td>`;
                        content += `</tr>`

                    }
                    else {
                    }
                    $("#thontindonhang").html(content);
                }
                else {
                    alert(r.message);
                }
            }
        });
    },

    LayLichSuDonHang: function () {
        jQuery.ajax({
            url: SiteUrl + "/taikhoan/LayLichSuDonHang",
            contentType: 'application/json',
            dataType: 'json',
            type: "GET",
            success: function (r) {
                let content = ``;
                if (r.status == true) {
                    if (r.data.length > 0) {
                        for (var i = 0; i < r.data.length; i++) {
                            content += `<tr>`
                            content += `<td align="center">` + (i + 1) + `</td>`;
                            content += `<td>` + r.data[i].madonhang + `</td>`;
                            content += `<td align="center">` + jsonUnixToDateTime(r.data[i].ngaydathang) + `</td>`;
                            content += `<td align="center">${numberToMoney(r.data[i].thanhtiensaugiam)}</td>`;
                            if (r.data[i].diachinhanhang != null) {
                                content += `<td align="center">` + r.data[i].diachinhanhang + `</td>`;
                            }
                            else {
                                content += `<td></td>`;
                            }
                            if (r.data[i].trangthai == true) {
                                content += `<td>Đã xử lý</td>`;
                            } else {
                                content += `<td>Chưa xử lý</td>`;
                            }
                            content += '<td align="center" style="width:5%"><a onclick="QUANCHI_TaiKhoan.OpenPopup(' + r.data[i].id + ')"><i class="far fa-eye"></i></a></td>';
                            content += `</tr>`
                        }
                    }
                }
                else {
                    alert(r.message);
                }
                $("#table-donhang").html(content);
            }
        });
    },

    ThemMoiGopYPhanHoi: function () {
        let gopy = jQuery('#gopy').val();
        let khieunai = jQuery('#khieunai').val();
        var pJson = { 'gopy': gopy, 'khieunai': khieunai };
        jQuery.ajax({
            url: SiteUrl + "/taikhoan/ThemMoiGopYPhanHoi",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (r) {
                if (r.status == true) {
                    infoShow('thongbao', 'Gửi thành công!', true);
                    $('#thongbao').css({ "color": "blue", "font-size": "10pt" });
                    $('.block-khdangnhap  .nav >li').removeClass('active');
                    $('#li_danhsachphanhoi').addClass('active');
                    $('#gopykhieunai').removeClass('fade in active');
                    $('#danhsachphanhoi').addClass('fade in active');
                    QUANCHI_TaiKhoan.ClearFrom();
                    QUANCHI_TaiKhoan.XemNoiDungPhanHoi();
                }
                if (r.status == false) {
                    infoShow('thongbao_loi', 'Gửi không thành công !', true);
                    $('#thongbao_loi').css({ "color": "red", "font-size": "10pt" });
                }
                
            },
            error: function (ex) {
                alertError('thongbao', 'Lỗi ' + ex);
            }
        });
    },

    ClearFrom: function () {
        jQuery('#gopy').val('');
        jQuery('#khieunai').val('');
    },

    XemNoiDungPhanHoi: function () {
        jQuery.ajax({
            url: SiteUrl + "/taikhoan/XemNoiDungPhanHoi",
            contentType: 'application/json',
            dataType: 'json',
            type: "GET",
            success: function (r) {
                let content = ``;
                if (r.status == true) {
                    if (r.data.length > 0) {
                        for (var i = 0; i < r.data.length; i++) {
                            content += `<tr>`
                            content += `<td align="center">` + (i + 1) + `</td>`;
                            content += `<td>` + r.data[i].noidunggopy + `</td>`;
                            content += `<td>` + r.data[i].noidungkhieunai + `</td>`;
                            content += `<td align="center">` + jsonUnixToDateTime(r.data[i].ngaygopy) + `</td>`;

                            if (r.data[i].noidungphanhoi != null) {
                                content += `<td>` + r.data[i].noidungphanhoi + `</td>`;
                            }
                            else {
                                content += `<td></td>`
                            }
                            content += `<td align="center">` + jsonUnixToDateTime(r.data[i].ngayphanhoi) + `</td>`;
                            content += `</tr>`
                        }
                    }
                }
                else {
                    alert(r.message);
                }
                $("#table_phanhoi").html(content);
            }
        });
    },

    GuiXacThucEmail: function () {
        let tendangnhap = jQuery('#tendangnhap').val();
        let email = jQuery('#emailxacthuc').val();
        if (email == "" || tendangnhap == "") {
            $("#thongbao").removeClass("hidden").text("Chưa nhập đủ thông tin");
            return;
        }
        var pJson = { email: email, tendangnhap: tendangnhap};
        jQuery.ajax({
            url: SiteUrl + "/taikhoan/GuiXacThucEmail",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (r) {
                if (r.status == true) {
                    $("#thongbao").removeClass("hidden").removeClass("text-danger").addClass("text-success").html(r.message);
                    $("#guixacnhan").prop('disabled', 1).html('<span id="timeout">(<span id="time">30</span>)</span> Gửi lại');
                    QUANCHI_TaiKhoan.Resend();
                }
                else {
                    $("#thongbao").removeClass("hidden").addClass("text-danger").removeClass("text-success").html(r.message);
                }
            },
            error: function (ex) {
                alertError('thongbao', 'Lỗi ' + ex);
            }
        });
    },

    Resend: function () {
        var timeout = 30;
        var i = setInterval(frame, 1000);
        function frame() {
            if (timeout == 0) {
                clearInterval(i);
                $("#guixacnhan").prop('disabled', 0);
                $("#timeout").remove();
            } else {
                timeout--;
                $("#time").text(timeout);
            }
        }
    },
}


function infoShow(spanid, message, autohide) {
    jQuery('#' + spanid).removeClass().html(message);
    jQuery('#' + spanid).show(200);
    if (autohide) {
        jQuery('#' + spanid).delay(2000).hide(1000);
    }
}