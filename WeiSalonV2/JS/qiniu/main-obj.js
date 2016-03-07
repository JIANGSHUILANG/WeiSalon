$(function() {
    $uparr = ['sllogo', 'slshow-one', 'slshow-two', 'slshow-three'];
    OQINIU.init($uparr);
});

var OQINIU = {
    init : function(list){
        var uparr = [];
        var _this = this;
        _this.domain = 'http://bobosquad.qiniudn.com';
        if(!$.isArray(list)) return false;

        for (var i = 0; i < list.length; i++) {

            ;(function(i){
                uparr[i] = Qiniu.uploader({
                    runtimes: 'html5,flash,html4',
                    browse_button: list[i],
                    container: list[i]+'-parent',
                    drop_element: list[i]+'-parent',
                    max_file_size: '100mb',
                    flash_swf_url: 'js/plupload/Moxie.swf',
                    dragdrop: true,
                    chunk_size: '4mb',
                    uptoken: _token,
                    domain: _this.domain,
                    auto_start: true,
                    init: {
                        'FileUploaded': function(up, file, info) {

                           _this.success(up, file, info,list[i]+'-parent');
                        },
                        'Error': function(up, err, errTip) {
                            _this.error();
                        },
                        'Key': function (up, file) {
                            var date = new Date();

                            var key = date.getFullYear().toString() + (date.getMonth() + 1).toString() + date.getDate().toString() + date.getHours().toString() + date.getMinutes().toString() + date.getSeconds().toString() + date.getMilliseconds().toString() + ".jpg";

                            return key;
                        }
                    }
                });
            })(i);
        };

    },
    success : function(up, file, info, parentid){
        var $parent = $('#'+parentid);
        var $info = $.parseJSON(info);
        console.log($info);
        console.log($parent);
        $parent.find('.up-img').attr('src', this.domain + '/' + $info["key"]).end();
        //$parent.find('.up-img').attr('src', this.domain + '/' + $info["key"]).end().find('.up-input').val('/' + $info["key"]);
    },
    error : function(){
        alert('上传失败，请重新上传！');
    }
}

