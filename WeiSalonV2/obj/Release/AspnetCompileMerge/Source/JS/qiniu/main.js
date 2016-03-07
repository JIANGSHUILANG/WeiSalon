/*global Qiniu */
/*global plupload */
/*global FileProgress */
/*global hljs */


$(function () {
    var domain = 'http://bobosquad.qiniudn.com';
    var aUploader = [];
    var aUploadFX = ['sllogo', 'slshow-md', 'slshow-one', 'slshow-two', 'slshow-three', 'B_Image1'
    , 'B_Image2', 'B_Image3', 'B_Image4', 'B_Image5', 'upload-z', 'upload-h1', 'upload-h2', 'upload-h3'];

    for (var i = 0; i < aUploadFX.length; i++) {

        ; (function (i) {
            aUploader[i] = Qiniu.uploader({
                runtimes: 'html5,flash,html4',
                browse_button: aUploadFX[i],
                container: aUploadFX[i] + '-parent',
                drop_element: aUploadFX[i] + '-parent',
                max_file_size: '100mb',
                flash_swf_url: 'js/plupload/Moxie.swf',
                dragdrop: true,
                chunk_size: '4mb',
                uptoken: _token,
                domain: domain,
                auto_start: true,
                init: {
                    'FileUploaded': function (up, file, info) {

                        upSuccess(up, file, info, aUploadFX[i] + '-parent');
                    },
                    'Error': function (up, err, errTip) {
                        upError(aUploadFX[i].id + '-parent');
                    },
                    'Key': function (up, file) {
                        var date = new Date();

                        var key = date.getFullYear().toString() + (date.getMonth() + 1).toString() + date.getDate().toString() + date.getHours().toString() + date.getMinutes().toString() + date.getSeconds().toString() + date.getMilliseconds().toString() + ".jpg";

                        return key;
                    }
                    /*,
                    'Key': function(up, file) {
                        var key = "";
                        return key
                    }
                    */
                }
            });
        })(i);
    };

    function upSuccess(up, file, info, parentid) {
        var oParent = $('#' + parentid);
        info = eval("(" + info + ")");
        if (!oParent.find('img').length) {
            oParent.append('<img src="" height="100">');
        }
        oParent.find('img').attr('src', domain + '/' + info["key"]);//.end().find('input[type=hidden]').val(info["key"]);


        ///如果是修改环境图等 则自动保存
        if (location.href.indexOf("/Manage/ImageInfo") != -1) {
            var sid = $("input[name=" + oParent.find('input').attr('name') + "]").val();
            $.ajax({
                url: "/Salon/UpdateImage",
                type: 'post',
                dataType: 'json',
                data: { id: sid, imagepath: "/" + info["key"] },
                success: function (data) {
                }
            });
        }
    }

    function upError(parentid) {
        alert('上传失败，请重新上传！');
    }
});
