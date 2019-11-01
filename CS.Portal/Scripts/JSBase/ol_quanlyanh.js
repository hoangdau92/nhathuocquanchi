var map; var wkt; var vectors; var boxLayer; var bounds; var idenControl;
var searchPolygon;
var footPrint = new Array();
var wtkInit;
var tinhLayer; var huyenLayer; var xaLayer;
var selectedFeature;
OpenLayers.IMAGE_RELOAD_ATTEMPTS = 5;
OpenLayers.DOTS_PER_INCH = 25.4 / 0.28;
var mapUrl = "http://10.151.130.87:8080/geoserver";
var attUrl = "http://10.151.130.87/vnredsat"
//jQuery(function() {
//    jQuery( "#fromDate" ).datepicker({ dateFormat: 'dd/mm/yy' });
//    jQuery( "#toDate" ).datepicker({ dateFormat: 'dd/mm/yy' });
//    jQuery( "#tabs" ).tabs({active:0});
//});
var selectControl;
var ol_init = {
    init: function () {
        ol_init.registerEvent();
    },
    registerEvent: function () {
        {
            searchPolygon = "";
            OpenLayers.Lang.setCode("vi");
            //bounds = new OpenLayers.Bounds(102.14391742043716, 7.886398431818287, 115.83922681964278, 23.39270280252601);
            bounds = new OpenLayers.Bounds(-126.99999999999997, -55.00000001886963, 141, 82.6274185352932);
            var format = 'image/png';
            var options = {
                controls: [],
                maxExtent: bounds,
                //maxResolution:0.0579162774609373,
                projection: "EPSG:4326",
                //        projection: "EPSG:32648",
                //        units: 'm'
                units: 'degrees'
            };
            map = new OpenLayers.Map('map', options);
            //ADD MAP LAYER
            tinhLayer = new OpenLayers.Layer.WMS(
                "Ranh giới tỉnh", mapUrl + "/vnredsat/wms",
                {
                    LAYERS: 'vnredsat:VN_HCtinh',
                    STYLES: '',
                    format: format,
                    transparent: true
                },
                {
                    singleTile: true,
                    ratio: 1,
                    isBaseLayer: false,
                    yx: { 'EPSG:4326': true }
                }
            );
            huyenLayer = new OpenLayers.Layer.WMS(
                "Ranh giới huyện", mapUrl + "/vnredsat/wms",
                {
                    LAYERS: 'vnredsat:VN_HChuyen',
                    STYLES: '',
                    format: format,
                    transparent: true
                },
                {
                    singleTile: true,
                    ratio: 1,
                    isBaseLayer: false,
                    yx: { 'EPSG:32648': true }
                }
            );
            xaLayer = new OpenLayers.Layer.WMS(
                "Ranh giới xã", mapUrl + "/vnredsat/wms",
                {
                    LAYERS: 'vnredsat:VN_HCxa',
                    STYLES: '',
                    format: format,
                    transparent: true
                },
                {
                    singleTile: true,
                    ratio: 1,
                    isBaseLayer: false,
                    yx: { 'EPSG:32648': true }
                }
            );
            //vnredsat:footprint
            var tempLayer = new OpenLayers.Layer.WMS(
                "Khung canh anh", mapUrl + "/vnredsat/wms",
                {
                    LAYERS: 'vnredsat:footprint',
                    STYLES: '',
                    format: format,
                    transparent: true
                },
                {
                    singleTile: false,
                    ratio: 1,
                    isBaseLayer: false,
                    yx: { 'EPSG:4326': true }
                }
            );
            //    var googleLayer = new OpenLayers.Layer.Google(
            //            "Google Streets", // the default
            //            {numZoomLevels: 20 }
            //        );

            //    var bingLayer = new OpenLayers.Layer.Bing({
            //        name: "Bing Aerial",
            //        key: 'Ak-dzM4wZjSqTlzveKz5u0d4IQ4bRzVI309GxmkgSVr1ewS6iPSrOvOKhA-CJlm3',
            //        type: "Aerial"
            //    });
            //countries
            var countries = new OpenLayers.Layer.WMS(
                "Thế giới", mapUrl + "/vnredsat/wms",
                {
                    LAYERS: 'vnredsat:countries',
                    STYLES: '',
                    format: format,
                    transparent: true
                },
                {
                    singleTile: true,
                    ratio: 1,
                    isBaseLayer: true,
                    yx: { 'EPSG:4326': true }
                }
            );
            wkt = new OpenLayers.Format.WKT();
            vectors = new OpenLayers.Layer.Vector("Vector Layer");
            boxLayer = new OpenLayers.Layer.Vector("Box layer");
            map.addLayers([xaLayer, huyenLayer, tinhLayer, vectors, boxLayer, countries, tempLayer]);

            //ADD MAP CONTROL
            map.addControl(new OpenLayers.Control.LayerSwitcher());
            map.zoomToExtent(bounds);
            // build up all controls
            map.addControl(new OpenLayers.Control.PanZoomBar({
                position: new OpenLayers.Pixel(2, 15)
            }));
            map.addControl(new OpenLayers.Control.Navigation());
            map.addControl(new OpenLayers.Control.Scale(document.getElementById('scale')));
            map.addControl(new OpenLayers.Control.MousePosition({ element: document.getElementById('location') }));

            selectControl = new OpenLayers.Control.SelectFeature(
                [vectors],
                {
                    clickout: true, toggle: false,
                    multiple: false, hover: false,
                    toggleKey: "ctrlKey"//, // ctrl key removes from selection
                    //multipleKey: "shiftKey" // shift key adds to selection
                }
            );

            vectors.events.register("featureselected", null, function (feature) {
                selectedFeature = feature;
                //alert(feature.feature.attribute.sqlid);
                ShowImageInfoPopup(feature.feature.attribute.sqlid);
            });
            map.addControl(selectControl);
            if (wtkInit != null && wtkInit != "") {
                parseWKT(wtkInit);
            }
            //draw control
            drawControls = {
                //        point: new OpenLayers.Control.DrawFeature(pointLayer,
                //                        OpenLayers.Handler.Point),
                //        line: new OpenLayers.Control.DrawFeature(lineLayer,
                //                        OpenLayers.Handler.Path),
                //        polygon: new OpenLayers.Control.DrawFeature(polygonLayer,
                //                        OpenLayers.Handler.Polygon),
                box: new OpenLayers.Control.DrawFeature(boxLayer,
                                OpenLayers.Handler.RegularPolygon, {
                                    handlerOptions: {
                                        sides: 4,
                                        irregular: true
                                    }
                                }
                            )
            };

            for (var key in drawControls) {
                map.addControl(drawControls[key]);
            }
            drawControls["box"].events.register("featureadded", ' ', OnFeatureAdded);
            //if (jQuery("#is_box").val() == "1") { drawControls["box"].activate(); }
            //else {
            //    drawControls["box"].deactivate();
            //}

        }
    }
}
ol_init.init();
function ShowImageInfoPopup(id) {
    jQuery('#myModal2').modal();
    jQuery("#myLoadingPopup2").attr("style", "display: block;");
    var ImgId = id;
    //Lấy dữ liệu
    var pJson = { 'ImageID': ImgId };
    jQuery.ajax({
        url: "/Home/ViewImageInfo",
        contentType: 'application/json',
        data: pJson,
        dataType: 'json',
        type: "GET",
        success: function (data) {
            if (!data[0]) {
                // alert('Không thể xem được dữ liệu!');
            }
            else {
                //console.log(data[0]);
                //alert(data[0].SceneDate);
                var MyDate_String_Value = data[0].SceneDate;
                var value = new Date
                            (
                                 parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                            );
                var day = value.getDate();
                var month = value.getMonth() + 1;
                var newdate = day + "/" + month + "/" + value.getFullYear();

                var strHtml = '<div class=\"well\" style=\"max-height: 420px;overflow: auto;\"><table class=\"table table-striped table-bordered table-hover\"><tr><th style=\"vertical-align:middle;text-align:center;\">Tên thuộc tính</th><th style=\"vertical-align:middle;text-align:center;\">Giá trị</th></tr>';
                jQuery("#lblTenAnh2").text(data[0].ImageName);
                strHtml += "<tr><td>Vệ tinh (Mission):</td><td>" + data[0].Mission + "</td></tr>";
                strHtml += "<tr><td>Độ phân giải (Resolution):</td><td>" + data[0].Revolution + "</td></tr>";
                strHtml += "<tr><td>Thời gian chụp ảnh (SceneDate):</td><td>" + newdate + "</td></tr>";
                strHtml += "<tr><td>Loại ảnh (Mode):</td><td>" + data[0].Mode + "</td></tr>";
                strHtml += "<tr><td>Độ phủ mây (Cloud):</td><td>" + data[0].Cloud + "</td></tr>";
                strHtml += "<tr><td>Chất lượng ảnh (Technical):</td><td>" + data[0].Technical + "</td></tr>";
                strHtml += "<tr><td rowspan='2' style='vertical-align:middle;'>Tọa độ trên trái (nwLat,nwLong):</td><td>" + data[0].NwLat + "</td></tr><tr><td>" + data[0].NwLong + "</td></tr>";
                strHtml += "<tr><td  rowspan='2' style='vertical-align:middle;'>Toạ độ trên phải (neLat,neLong):</td><td>" + data[0].NeLat + "</td></tr><tr><td>" + data[0].NeLong + "</td></tr>";
                strHtml += "<tr><td  rowspan='2' style='vertical-align:middle;'>Toạ độ dưới trái (swLat,swLong):</td><td>" + data[0].SwLat + "</td></tr><tr><td>" + data[0].SwLong + "</td></tr>";
                strHtml += "<tr><td  rowspan='2' style='vertical-align:middle;'>Toạ độ dưới phải (seLat,seLong):</td><td>" + data[0].SeLat + "</td></tr><tr><td>" + data[0].SeLong + "</td></tr>";
                strHtml += "<tr><td  rowspan='2' style='vertical-align:middle;'>Tọa độ tâm (centerLat,centerLong):</td><td>" + data[0].CenterLat + "</td></tr><tr><td>" + data[0].CenterLong + "</td></tr>";
                strHtml += "</table></div><button type=\"button\" class=\"btn btn-default pull-right\" data-dismiss=\"modal\">Đóng</button></br>";
                jQuery('#pChiTietMetaData2').html(strHtml);
                var strImage = "<img  src=\"/Images/PreviewImg/" + data[0].URLImgPre + "\" style=\"height:320px;width:300px;border-width:0px;\">";
                jQuery('#pAnhXemNhanh2').html(strImage);
                jQuery("#myLoadingPopup2").attr("style", "display: none;");
            }
        }
    });
}
function OnFeatureAdded(data) {
    searchPolygon = data.feature.geometry.bounds.toGeometry();
}

function toggleControl(isActive) {
    searchPolygon = "";
    if (isActive == true) {
        drawControls["box"].activate();
    }
    else {
        drawControls["box"].deactivate();
    }
}

function TimTheoDoHoa() {
    var loaiTimKiem = jQuery("#is_box").val();
    if (loaiTimKiem == "0") {
        toggleControl(false);
        boxLayer.destroyFeatures();
        jQuery("#divDVHC").hide();
    }
    else {
        if (loaiTimKiem == "2") {
            if (jQuery("#search_dvhc").find('option').length < 1) {

                //load danh sach don vi hanh chinh
                var tinhList = jQuery.ajax({
                    type: "POST",
                    url: attUrl + "/WSMap.asmx/GetDVHC",
                    crossDomain: true,
                    async: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json"
                    //success: function(msg) {

                    //}
                });
                var returnArr = new OpenLayers.Format.JSON().read(tinhList.responseJSON.d);
                var html = "";
                if (returnArr.length > 0) {
                    html += "<option value='0'>Lựa chọn tỉnh</option>";
                    for (var i = 0; i < returnArr.length; i++) {
                        var matinh = "";
                        if (returnArr[i].ma_dvhc_t == null) {
                            matinh = "-1";
                        }
                        else {
                            matinh = returnArr[i].ma_dvhc_t;
                        }
                        html += "<option value='" + matinh + "'>" + returnArr[i].ten_dvhc_t + "</option>";
                    }
                    jQuery("#search_dvhc").html(html);
                }
            }

            jQuery("#divDVHC").show();
            jQuery("#search_dvhc").width(100);
            jQuery("#search_dvhc").val("0");
        }
        else {
            jQuery("#divDVHC").hide();
            toggleControl(true);
        }
    }
}

function onDVHCChange() {
    var idTinh = jQuery("#search_dvhc").val();
    //load danh sach xa theo tinh
    if (idTinh == "0" || idTinh == "-1") { }
    else {
        var xaList = jQuery.ajax({
            type: "POST",
            url: attUrl + "/WSMap.asmx/GetXaByTinh",
            crossDomain: true,
            async: false,
            data: JSON.stringify({
                idTinh: idTinh
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });
        var returnArr = new OpenLayers.Format.JSON().read(xaList.responseJSON.d);
        var html = "";
        if (returnArr.length > 0) {
            for (var i = 0; i < returnArr.length; i++) {
                html += "<option value='" + returnArr[i].ma_dvhc_x + "'>" + returnArr[i].ten_dvhc_x + "</option>";
            }
            jQuery("#search_xa").append(html);
        }
    }
}

function timeConverter(UNIX_timestamp) {
    var a = new Date(UNIX_timestamp);
    var months = ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'];
    var year = a.getFullYear();
    var month = months[a.getMonth()];
    var date = a.getDate();
    var hour = a.getHours();
    var min = a.getMinutes();
    var sec = a.getSeconds();
    var time = date + ',' + month + ' ' + year + ' ' + hour + ':' + min + ':' + sec;
    return time;
}

function ZoomToFP() {
    map.zoomToExtent(selectedFeature.feature.geometry.getBounds());
}

function ClearThongTinThuocTinh() {

    jQuery("tencanhanh").text("");
    jQuery("#lblimagename").text("");
    jQuery("#lblmission").text("");
    jQuery("#lblresolution").text("");
    jQuery("#lblscenedate").text("");
    jQuery("#lblmode").text("");
    jQuery("#lblcloud").text("");
    jQuery("#lbltechnical").text("");
    jQuery("#lblnw").text("");
    jQuery("#lblne").text("");
    jQuery("#lblsw").text("");
    jQuery("#lblse").text("");
    jQuery("#lblcenter").text("");
    jQuery("#anhxemnhanh").attr("src", "");
    jQuery("#thongtinthuoctin").hide();
    jQuery("#thongbao").text("");
    selectedFeature = null;
    jQuery("#tencanhanh").val("");
    vectors.destroyFeatures();
    boxLayer.destroyFeatures();
}

function HuyKetQuaTimKiem() {
    ClearThongTinThuocTinh();
    map.zoomToExtent(bounds);
}
function TimKiem() {
    var loaiTimKiem = jQuery("#is_box").val();
    if (loaiTimKiem != "2") { //tim thuoc tinh hoac do hoa
        var dophumay = jQuery("#phumay").val();
        var dophangiai = jQuery("#phangiai").val();
        var chatluonganh = jQuery("#chatluong").val();
        var loaianh = jQuery("#loaianh").val();
        var tungay = jQuery("#fromDate").val();
        var denngay = jQuery("#toDate").val();
        var tencanhanh = jQuery('#tencanhanh').val();
        ClearThongTinThuocTinh();
        var is_box = "0";

        if (jQuery("#is_box").val() == "1" && searchPolygon != "") {
            is_box = searchPolygon;
        }

        jQuery.ajax({
            type: "POST",
            url: attUrl + "/WSMap.asmx/HelloWorld",
            data: JSON.stringify({
                dophumay: dophumay,
                dophangiai: dophangiai,
                chatluonganh: chatluonganh,
                loaianh: loaianh,
                tungay: tungay,
                denngay: denngay,
                tencanhanh: tencanhanh,
                is_box: "'" + is_box + "'"
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var returnArr = new OpenLayers.Format.JSON().read(msg.d);
                if (returnArr.length > 0) {
                    var html = '';
                    html += '<table>';
                    html += '<tr>';
                    html += '<th>';
                    html += 'Tên cảnh ảnh';
                    html += '</th>';
                    html += '<th>';
                    html += 'Xem chi tiết';
                    html += '</th>';
                    html += '<tr>';

                    for (var i = 0; i < returnArr.length; i++) {
                        /* zoom to feature
                        var footPrintWTK = returnArr[i].footPrintWTK;

                        if (footPrintWTK != null && footPrintWTK != "") {
                        var features = wkt.read(footPrintWTK);
                        features.attribute = new Object();
                        features.attribute.sqlid = returnArr[i].ID;
                        vectors.addFeatures(features);
                        }
                        */
                        html += "<tr>";
                        html += "<td>" + returnArr[i].Name + "</td>";
                        html += "<td><span onclick='zoomToFeature(" + returnArr[i].ID + ")'>Chi tiết</span></td>";
                        html += "</tr>";
                    }
                    html += '</table>';
                    jQuery("#thongbao").text("Tìm được " + (returnArr.length) + " ảnh viễn thám trong CSDL.");
                    jQuery("#Div1").html(html);
                    /*map.zoomToExtent(vectors.getDataExtent());
                    selectControl.activate();*/
                }
                else {
                    jQuery("#thongbao").text("Không tìm thấy ảnh viễn thám trong CSDL.");
                }

            }
        });
    }
    else { //tìm kiếm theo đơn vị hành chính
        ClearThongTinThuocTinh();
        var gid_dvhc = jQuery("#search_dvhc").val();
        if (gid_dvhc != "0") {
            var a = jQuery.ajax({
                type: "POST",
                url: attUrl + "/WSMap.asmx/SearchDVHC",
                data: JSON.stringify({
                    gid_dvhc: gid_dvhc,
                    stype: "0"
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var returnArr = new OpenLayers.Format.JSON().read(msg.d);
                    if (returnArr.length > 0) {
                        /*
                        //neu tim kiem zoomto luon don vi hanh chin
                        for (var i = 0; i < returnArr.length; i++) {
                        var footPrintWTK = returnArr[i].st_astext;

                        if (footPrintWTK != null && footPrintWTK != "") {
                        var features = wkt.read(footPrintWTK);
                        features.attribute = new Object();
                        features.attribute.sqlid = returnArr[i].attid;
                        vectors.addFeatures(features);
                        }
                        }
                        jQuery("#thongbao").text("Tìm được " + (returnArr.length) + " ảnh viễn thám trong CSDL.");
                        map.zoomToExtent(vectors.getDataExtent());
                        selectControl.activate();
                        //fill du lieu vao danh sach ket qua tim kiem
                        */
                        //hien thi danh sach
                        var html = '';
                        html += '<table>';
                        html += '<tr>';
                        html += '<th>';
                        html += 'Tên cảnh ảnh';
                        html += '</th>';
                        html += '<th>';
                        html += 'Xem chi tiết';
                        html += '</th>';
                        html += '<tr>';

                        for (var i = 0; i < returnArr.length; i++) {
                            /* zoom to feature
                            var footPrintWTK = returnArr[i].footPrintWTK;

                        if (footPrintWTK != null && footPrintWTK != "") {
                            var features = wkt.read(footPrintWTK);
                            features.attribute = new Object();
                            features.attribute.sqlid = returnArr[i].ID;
                            vectors.addFeatures(features);
                            }
                            */
                            html += "<tr>";
                            html += "<td>" + returnArr[i].Name + "</td>";
                            html += "<td><span onclick='zoomToFeature(" + returnArr[i].ID + ")'>Chi tiết</span></td>";
                            html += "</tr>";
                        }
                        html += '</table>';
                        jQuery("#thongbao").text("Tìm được " + (returnArr.length) + " ảnh viễn thám trong CSDL.");
                        jQuery("#Div1").html(html);
                    }
                    else {
                        jQuery("#thongbao").text("Không tìm thấy ảnh viễn thám trong CSDL.");
                    }
                }
            });


        }
        else {
            alert("Phải lựa chọn đơn vị hành chính để tìm kiếm!");
        }
    }
}

function zoomToFeature(attID) {
    vectors.destroyFeatures();
    jQuery.ajax({
        type: "POST",
        url: "/Home/GetFeatureByAttID",
        data: JSON.stringify({
            attID: attID
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = msg.data;
            var footPrintWTK = data[0].st_astext;
            var features = wkt.read(footPrintWTK);
            features.attribute = new Object();
            features.attribute.sqlid = data[0].id;
            vectors.addFeatures(features);
            map.zoomToExtent(vectors.getDataExtent());
            selectControl.activate();
        }
    });
}

function zoomToMultipeFeature() {
    //Clear all feature
    vectors.destroyFeatures();
    var attID = "";
    //Get all checkbox checked
    jQuery('input[name="chk_zoom"]:checked').each(function () {
        attID = jQuery(this).val();
        jQuery.ajax({
            type: "POST",
            url: "/Home/GetFeatureByAttID",
            data: JSON.stringify({
                attID: attID
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var data = msg.data;
                var footPrintWTK = data[0].st_astext;
                var features = wkt.read(footPrintWTK);
                features.attribute = new Object();
                features.attribute.sqlid = data[0].id;
                vectors.addFeatures(features);
            }
        });
    });
    console.log(vectors);
    map.zoomToExtent(vectors.getDataExtent());
}
function parseWKT(strWKT) {
    var features = wkt.read(strWKT);
    var bounds;
    if (features) {
        if (features.constructor != Array) {
            features = [features];
        }
        for (var i = 0; i < features.length; ++i) {
            if (!bounds) {
                bounds = features[i].geometry.getBounds();
            } else {
                bounds.extend(features[i].geometry.getBounds());
            }
        }
        vectors.addFeatures(features);
        map.zoomToExtent(bounds);
        jQuery("#thongtinthuoctin").show();
        console.log(assignAtt);

        jQuery("#lblimagename").text(assignAtt.Name);
        jQuery("#lblmission").text(assignAtt.Mission);
        jQuery("#lblresolution").text(assignAtt.Revolution);

        jQuery("#lblscenedate").text(assignAtt.SceneDate);
        jQuery("#lblmode").text(assignAtt.Mode);
        jQuery("#lblcloud").text(assignAtt.Cloud);
        jQuery("#lbltechnical").text(assignAtt.Technical);
        jQuery("#lblnw").text(assignAtt.NwLat + " " + assignAtt.NwLong);
        jQuery("#lblne").text(assignAtt.NeLat + " " + assignAtt.NeLong);
        jQuery("#lblsw").text(assignAtt.SwLat + " " + assignAtt.SwLong);
        jQuery("#lblse").text(assignAtt.SeLat + " " + assignAtt.SeLong);
        jQuery("#lblcenter").text(assignAtt.CenterLat + "" + assignAtt.CenterLong);
        var src = assignAtt.URLImgPre + ".jpg";
        jQuery("#anhxemnhanh").attr("src", src);

        jQuery("#tencanhanh").val(assignAtt.Name);
    } else {
        var result = 'Bad WKT';
    }
}

function CreateArr(footprint_geom, rect_geom, attid) {
    footPrint[footPrint.length] = { footprint_geom: footprint_geom, rect_geom: rect_geom, attid: attid };
}