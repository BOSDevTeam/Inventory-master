function showLoadingIcon() {
    var loadingImage = '<div class="loading-div">' +
      '<span class="loading-icon " style="opacity:none;">' +
      '<i class="fa fa-spin fa-2x" style="margin-left:47px;"></i>' +
      '</span></div>';
    $('body').append(loadingImage);
}
function hideLoadingIcon() {       //call this when you want to hide loading icon
    $('.loading-div').remove();
}
