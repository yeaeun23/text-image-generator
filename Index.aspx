<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>전광판 주요뉴스 관리</title>
    <script type="text/javascript" src="js/jquery-3.5.0.min.js"></script>
    <script type="text/javascript" src="js/huebee.pkgd.js"></script>
    <script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js"></script>
    <link type="text/css" rel="stylesheet" href="css/styles.css" />
    <link type="text/css" rel="stylesheet" href="css/huebee.css" />
    <link type="text/css" rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" />
</head>

<body>
    <form id="form1" runat="server">
        <main class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12">
                    <div class="button" style="float: left;">
                        <button runat="server" id="FileSelect_DispNone" onserverclick="FileSelect_DispNone_Click"></button>
                        <input runat="server" type="file" id="FileSelect" multiple="multiple" accept=".jpg" onchange="return fileSelect_onChange();" />
                        <label class="btn btn-sm btn-primary" for="FileSelect">파일 선택(속보#.jpg)</label>

                        <button type="button" class="btn btn-sm btn-primary" data-toggle="modal" data-target="#myModal">파일 생성</button>

                        <asp:Button runat="server" ID="FileUploadAll" CssClass="btn btn-sm btn-danger" OnClientClick="return confirm_uploadAll();" OnClick="FileUploadAll_Click" Text="전체 전송" />
                    </div>

                    <div class="button" style="float: right;">
                        <a class="btn btn-sm btn-secondary" href="http://123.140.28.167:9080/app/live/sim/single.asp" target="_blank">전광판 카메라(IE)</a>
                        <asp:Button runat="server" ID="FileRefresh" CssClass="btn btn-sm btn-primary" OnClick="FileRefresh_Click" Text="새로고침" />
                        <asp:Button runat="server" ID="FileSync" CssClass="btn btn-sm btn-danger" OnClientClick="return confirm_sync();" OnClick="FileSync_Click" Text="동기화" />
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12 col-sm-12">
                    <div class="card h-100">
                        <div class="card-header">주요뉴스 전송</div>

                        <ul class="list-unstyled p-2 d-flex flex-column col files">
                            <li runat="server" id="FileError0" class="text-danger text-center">오류가 발생했습니다.</li>
                            <li runat="server" id="FileEmpty0" class="text-muted text-center">전송할 파일을 선택하세요.</li>

                            <asp:Repeater runat="server" ID="UploadFileRepeater">
                                <ItemTemplate>
                                    <li class="media">
                                        <div class="media-body mb-1">
                                            <img src='<%# string.Format("./{0}?{1}", Eval("path"), DateTime.Now.ToString("yyyyMMddHHmmss")) %>' />
                                            <strong><%# Eval("name") %></strong>
                                            <div class='button <%# Eval("state") %>' style="float: right;">
                                                <div class="progress">
                                                    <div class="progress-bar progress-bar-striped progress-bar-animated bg-danger" style="width: 100%;"></div>
                                                </div>
                                                <asp:Button runat="server" CssClass="btn btn-sm btn-light" Text="취소" CommandArgument='<%# Eval("name") %>' OnCommand="SvrFileDelete_Click" />
                                            </div>
                                            <hr class="mt-1 mb-1" />
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6 col-sm-12">
                    <div class="card h-100">
                        <div class="card-header">주요뉴스 (시청 방향)</div>

                        <ul id="svrFileList1" class="list-unstyled p-2 d-flex flex-column col files">
                            <li runat="server" id="FileError1" class="text-danger text-center">오류가 발생했습니다.</li>
                            <li runat="server" id="FileEmpty1" class="text-muted text-center">전송된 파일이 없습니다.</li>

                            <asp:Repeater runat="server" ID="SvrFileRepeater1">
                                <ItemTemplate>
                                    <li class="media">
                                        <div class="media-body mb-1">
                                            <p class="mb-2">
                                                <strong><%# Eval("name") %></strong>
                                                <span class="text-muted">(<%# Eval("time") %>)</span>
                                            </p>
                                            <img src='<%# string.Format("./{0}?{1}", Eval("path"), DateTime.Now.ToString("yyyyMMddHHmmss")) %>' />
                                            <hr class="mt-1 mb-1">
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </div>

                <div class="col-md-6 col-sm-12">
                    <div class="card h-100">
                        <div class="card-header">주요뉴스 (광화문 방향)</div>

                        <ul id="svrFileList2" class="list-unstyled p-2 d-flex flex-column col files">
                            <li runat="server" id="FileError2" class="text-danger text-center">오류가 발생했습니다.</li>
                            <li runat="server" id="FileEmpty2" class="text-muted text-center">전송된 파일이 없습니다.</li>

                            <asp:Repeater runat="server" ID="SvrFileRepeater2">
                                <ItemTemplate>
                                    <li class="media">
                                        <div class="media-body mb-1">
                                            <p class="mb-2">
                                                <strong><%# Eval("name") %></strong>
                                                <span class="text-muted">(<%# Eval("time") %>)</span>
                                            </p>
                                            <img src='<%# string.Format("./{0}?{1}", Eval("path"), DateTime.Now.ToString("yyyyMMddHHmmss")) %>' />
                                            <hr class="mt-1 mb-1">
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </div>
            </div>
        </main>

        <!-- The Modal -->
        <div class="modal fade" id="myModal">
            <div class="modal-dialog modal-dialog-centered modal-xl">
                <div class="modal-content">
                    <!-- Modal Header -->
                    <div class="modal-header">
                        <h5 class="modal-title">파일 생성</h5>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>

                    <!-- Modal body -->
                    <div class="modal-body">
                        <table>
                            <colgroup>
                                <col width="10%" />
                                <col width="24%" />
                                <col width="16%" />
                                <col width="10%" />
                                <col width="24%" />
                                <col width="16%" />
                            </colgroup>
                            <tr>
                                <th>
                                    <label class="col-form-label-sm" for="FileName">파일명</label>
                                </th>
                                <td colspan="5">
                                    <div class="form-check-inline">
                                        <input runat="server" type="radio" id="FileName1" class="form-check-input" name="filename" value="속보1" />
                                        <label class="form-check-label col-form-label-sm" for="FileName1">속보 1</label>
                                    </div>
                                    <div class="form-check-inline">
                                        <input runat="server" type="radio" id="FileName2" class="form-check-input" name="filename" value="속보2" />
                                        <label class="form-check-label col-form-label-sm" for="FileName2">속보 2</label>
                                    </div>
                                    <div class="form-check-inline">
                                        <input runat="server" type="radio" id="FileName3" class="form-check-input" name="filename" value="속보3" />
                                        <label class="form-check-label col-form-label-sm" for="FileName3">속보 3</label>
                                    </div>
                                    <div class="form-check-inline">
                                        <input runat="server" type="radio" id="FileName4" class="form-check-input" name="filename" value="속보4" />
                                        <label class="form-check-label col-form-label-sm" for="FileName4">속보 4</label>
                                    </div>
                                    <div class="form-check-inline">
                                        <input runat="server" type="radio" id="FileName5" class="form-check-input" name="filename" value="속보5" />
                                        <label class="form-check-label col-form-label-sm" for="FileName5">속보 5</label>
                                    </div>
                                    <div class="form-check-inline">
                                        <input runat="server" type="radio" id="FileName6" class="form-check-input" name="filename" value="속보6" />
                                        <label class="form-check-label col-form-label-sm" for="FileName6">속보 6</label>
                                    </div>
                                    <div class="form-check-inline">
                                        <input runat="server" type="radio" id="FileName7" class="form-check-input" name="filename" value="속보7" />
                                        <label class="form-check-label col-form-label-sm" for="FileName7">속보 7</label>
                                    </div>
                                    <div class="form-check-inline">
                                        <input runat="server" type="radio" id="FileName8" class="form-check-input" name="filename" value="속보8" />
                                        <label class="form-check-label col-form-label-sm" for="FileName8">속보 8</label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label class="col-form-label-sm" for="Contents">내용</label>
                                </th>
                                <td colspan="4">
                                    <input type="text" id="Contents" class="form-control form-control-sm" />
                                </td>
                                <td style="padding-left: 0px">
                                    <button type="button" class="btn btn-sm btn-light ym">“</button>
                                    <button type="button" class="btn btn-sm btn-light ym">”</button>
                                    <button type="button" class="btn btn-sm btn-light ym">‘</button>
                                    <button type="button" class="btn btn-sm btn-light ym">’</button>
                                    <button type="button" class="btn btn-sm btn-light ym">·</button>
                                    <button type="button" class="btn btn-sm btn-light ym">…</button>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label class="col-form-label-sm" for="FontFamily">글꼴</label>
                                </th>
                                <td colspan="2">
                                    <select class="form-control form-control-sm" id="FontFamily" disabled="disabled">
                                        <option>서울신문태고딕유니</option>
                                    </select>
                                </td>
                                <th>
                                    <label class="col-form-label-sm" for="FontColor">글꼴 색</label>
                                </th>
                                <td colspan="2">
                                    <input type="text" id="FontColor" class="form-control form-control-sm huebee" value="#FFFFFF" />
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label class="col-form-label-sm" for="FontSize">글꼴 크기(pt)</label>
                                </th>
                                <td>
                                    <input type="number" id="FontSize" class="form-control form-control-sm" value="42" disabled="disabled" />
                                </td>
                                <td style="padding-left: 0px">
                                    <div class="form-check-inline">
                                        <input type="checkbox" id="FontSizeDefault" class="form-check-input" checked="checked" />
                                        <label class="form-check-label col-form-label-sm" for="FontSizeDefault">기본: 42</label>
                                    </div>
                                </td>
                                <th>
                                    <label class="col-form-label-sm" for="FontSpace">자간</label>
                                </th>
                                <td>
                                    <input type="number" id="FontSpace" class="form-control form-control-sm" value="-20" disabled="disabled" />
                                </td>
                                <td style="padding-left: 0px">
                                    <div class="form-check-inline">
                                        <input type="checkbox" id="FontSpaceDefault" class="form-check-input" checked="checked" />
                                        <label class="form-check-label col-form-label-sm" for="FontSpaceDefault">기본: -20</label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label class="col-form-label-sm" for="PosX">수평 위치</label>
                                </th>
                                <td>
                                    <input type="number" id="PosX" class="form-control form-control-sm" value="220.0" disabled="disabled" />
                                </td>
                                <td style="padding-left: 0px">
                                    <div class="form-check-inline">
                                        <input type="checkbox" id="PosXCenter" class="form-check-input" checked="checked" />
                                        <label class="form-check-label col-form-label-sm" for="PosXCenter">가운데</label>
                                    </div>
                                </td>
                                <th>
                                    <label class="col-form-label-sm" for="PosY">수직 위치</label>
                                </th>
                                <td>
                                    <input type="number" id="PosY" class="form-control form-control-sm" value="11.375" disabled="disabled" />
                                </td>
                                <td style="padding-left: 0px">
                                    <div class="form-check-inline">
                                        <input type="checkbox" id="PosYCenter" class="form-check-input" checked="checked" />
                                        <label class="form-check-label col-form-label-sm" for="PosYCenter">가운데: 11.375</label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <label class="col-form-label-sm" for="PreviewImg">미리보기</label>
                                </th>
                                <td colspan="5">
                                    <img id="PreviewImg" src="bg.jpg" />
                                </td>
                            </tr>
                        </table>
                    </div>

                    <!-- Modal footer -->
                    <div class="modal-footer">
                        <p id="PreviewErrMsg" class="text-danger" style="float: left;"></p>

                        <div class="button" style="float: right;">
                            <asp:Button runat="server" ID="SendPreviewBtn" CssClass="btn btn-sm btn-primary" OnClientClick="return send_previewImg();" OnClick="SendPreviewBtn_Click" Text="파일 생성 완료" />
                            <button type="button" class="btn btn-sm btn-secondary" data-dismiss="modal">취소</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <script type="text/javascript">
            // 파일 선택
            function fileSelect_onChange() {
                $('#FileSelect_DispNone').click();
            }

            // 전체 전송
            function confirm_uploadAll() {
                if (confirm('전체 전송하시겠습니까?'))
                    return true;
                else
                    return false;
            }

            // 동기화
            function confirm_sync() {
                if (confirm('주요뉴스(시청/광화문 방향)를 최신 파일로 동기화하시겠습니까?'))
                    return true;
                else
                    return false;
            }

            // 파일 생성 - 파일 생성 완료
            function send_previewImg() {
                if (check_previewInput()) {
                    return true;
                }

                return false;
            }

            // 파일 생성 - 빈칸 확인
            function check_previewInput() {
                if ($("#Contents").val() == "") {
                    $("#Contents").focus();
                    $("#PreviewErrMsg").html("'내용'을(를) 입력해 주세요.");
                    return false;
                }
                else if ($("#FontFamily").val() == "") {
                    $("#FontFamily").focus();
                    $("#PreviewErrMsg").html("'글꼴'을(를) 선택해 주세요.");
                    return false;
                }
                else if ($("#FontColor").val() == "") {
                    $("#FontColor").focus();
                    $("#PreviewErrMsg").html("'글꼴 색'을(를) 선택해 주세요.");
                    return false;
                }
                else if ($("#FontSize").val() == "" && !$("input:checkbox[id='FontSizeDefault']").is(":checked")) {
                    $("#FontSize").focus();
                    $("#PreviewErrMsg").html("'글꼴 크기'을(를) 입력해 주세요.");
                    return false;
                }
                else if ($("#FontSpace").val() == "" && !$("input:checkbox[id='FontSpaceDefault']").is(":checked")) {
                    $("#FontSpace").focus();
                    $("#PreviewErrMsg").html("'자간'을(를) 입력해 주세요.");
                    return false;
                }
                else if ($("#PosX").val() == "" && !$("input:checkbox[id='PosXCenter']").is(":checked")) {
                    $("#PosX").focus();
                    $("#PreviewErrMsg").html("'수평 위치'을(를) 입력해 주세요.");
                    return false;
                }
                else if ($("#PosY").val() == "" && !$("input:checkbox[id='PosYCenter']").is(":checked")) {
                    $("#PosY").focus();
                    $("#PreviewErrMsg").html("'수직 위치'을(를) 입력해 주세요.");
                    return false;
                }
                else if (!$("input:radio[name='filename']").is(':checked')) {
                    $("#PreviewErrMsg").html("'파일명'을(를) 선택해 주세요.");
                    return false;
                }

                return true;
            }

            // 파일 생성 - 미리보기
            function make_previewImg() {
                $("#PreviewErrMsg").html("");

                // 내용
                var contents = $("#Contents").val();

                // 글꼴
                var fontFamily = $("#FontFamily").val();

                // 글꼴 색
                var fontColor = $("#FontColor").val();

                // 글꼴 크기
                if ($("input:checkbox[id='FontSizeDefault']").is(":checked"))
                    var fontSize = "default";
                else
                    var fontSize = $("#FontSize").val();

                // 자간
                if ($("input:checkbox[id='FontSpaceDefault']").is(":checked"))
                    var fontSpace = "default";
                else
                    var fontSpace = $("#FontSpace").val();

                // 수평 위치
                if ($("input:checkbox[id='PosXCenter']").is(":checked"))
                    var posX = "center";
                else
                    var posX = $("#PosX").val();

                // 수직 위치
                if ($("input:checkbox[id='PosYCenter']").is(":checked"))
                    var posY = "center";
                else
                    var posY = $("#PosY").val();

                // 파일 생성
                if (fontFamily != "" && fontColor != "" && fontSize != "" && fontSpace != "" && posX != "" && posY != "") {
                    $.ajax({
                        url: 'Preview.aspx',
                        method: 'post',
                        data: "contents=" + contents + "&fontFamily=" + fontFamily + "&fontColor=" + fontColor + "&fontSize=" + fontSize + "&fontSpace=" + fontSpace + "&posX=" + posX + "&posY=" + posY,
                        success: function (data) {
                            $("#PreviewImg").attr("src", "preview.jpg?" + new Date().getTime());
                            return true;
                        },
                        //error: function(request,status,error){
                        //    alert("code:"+request.status+"\n"+"message:"+request.responseText+"\n"+"error:"+error);
                        //}
                    })
                }
                else {
                    return false;
                }
            }

            $(function () {
                // 전송 실패
                $('.button').each(function () {
                    if ($(this).hasClass('fail'))
                        $(this).children('.progress').children('.progress-bar').html('실패');
                });

                // 스크롤
                $('#svrFileList1').scroll(function () {
                    $('#svrFileList2').scrollTop($('#svrFileList1').scrollTop());
                    $('#svrFileList2').scrollLeft($('#svrFileList1').scrollLeft());
                });

                $('#svrFileList2').scroll(function () {
                    $('#svrFileList1').scrollTop($('#svrFileList2').scrollTop());
                    $('#svrFileList1').scrollLeft($('#svrFileList2').scrollLeft());
                });

                // 파일 생성 - Color Picker
                var elem = $('.huebee')[0];
                var hueb = new Huebee(elem, {
                    notation: 'hex',
                    saturations: 2
                });
                var _gaq = _gaq || [];
                _gaq.push(['_setAccount', 'UA-36251023-1']);
                _gaq.push(['_setDomainName', 'jqueryscript.net']);
                _gaq.push(['_trackPageview']);

                (function () {
                    var ga = document.createElement('script');
                    ga.type = 'text/javascript';
                    ga.async = true;
                    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';

                    var s = document.getElementsByTagName('script')[0];
                    s.parentNode.insertBefore(ga, s);
                })();

                // 파일 생성 - 입력
                // 내용
                $("#Contents").on("propertychange change keyup cut copy paste input", function () {
                    make_previewImg();
                });

                // 약물
                $('.ym').on("click", function () {
                    var cursorPos = $('#Contents').prop('selectionStart');
                    var text = $('#Contents').val();
                    var textBefore = text.substring(0, cursorPos);
                    var textAfter = text.substring(cursorPos, text.length);

                    $('#Contents').val(textBefore + $(this).html() + textAfter);
                    $('#Contents').prop('selectionStart', cursorPos + $(this).html().length);
                    $('#Contents').prop('selectionEnd', cursorPos + $(this).html().length);
                    $('#Contents').focus();

                    make_previewImg();
                });

                // 글꼴
                $("#FontFamily").on("propertychange change keyup cut copy paste input", function () {
                    make_previewImg();
                });

                // 글꼴 크기
                $("#FontSize").on("propertychange change keyup cut copy paste input", function () {
                    make_previewImg();
                });

                // 글꼴 크기 CB
                $("#FontSizeDefault").on("change", function () {
                    if ($("input:checkbox[id='FontSizeDefault']").is(":checked"))
                        $("#FontSize").prop("disabled", true);
                    else
                        $("#FontSize").prop("disabled", false);

                    make_previewImg();
                });

                // 자간
                $("#FontSpace").on("propertychange change keyup cut copy paste input", function () {
                    make_previewImg();
                });

                // 자간 CB
                $("#FontSpaceDefault").on("change", function () {
                    if ($("input:checkbox[id='FontSpaceDefault']").is(":checked"))
                        $("#FontSpace").prop("disabled", true);
                    else
                        $("#FontSpace").prop("disabled", false);

                    make_previewImg();
                });

                // 수평 위치
                $("#PosX").on("propertychange change keyup cut copy paste input", function () {
                    make_previewImg();
                });

                // 수평 위치 CB
                $("#PosXCenter").on("change", function () {
                    if ($("input:checkbox[id='PosXCenter']").is(":checked"))
                        $("#PosX").prop("disabled", true);
                    else
                        $("#PosX").prop("disabled", false);

                    make_previewImg();
                });

                // 수직 위치
                $("#PosY").on("propertychange change keyup cut copy paste input", function () {
                    make_previewImg();
                });

                // 수직 위치 CB
                $("#PosYCenter").on("change", function () {
                    if ($("input:checkbox[id='PosYCenter']").is(":checked"))
                        $("#PosY").prop("disabled", true);
                    else
                        $("#PosY").prop("disabled", false);

                    make_previewImg();
                });
            });
        </script>
    </form>
</body>

</html>
