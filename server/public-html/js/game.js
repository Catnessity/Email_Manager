document.addEventListener('DOMContentLoaded', event =>{
   
    gameLoop();  // Start the loop
});

async function gameLoop() {
    await setupGame();  // Wait for the async setupGame to complete
    setTimeout(() => {
        resetGame();  // After 5 seconds, reset the game
        gameLoop();   // Then restart the loop
        setTimeout(() => {}, 500);
    }, 5000);
}

function resetGame(){
    document.getElementById("DownloadGame").innerHTML="";
    document.getElementById("DownloadGame").style.display= "none";
}

async function setupGame(){
    let gameboard = document.getElementById("DownloadGame");
    for(i=0; i<=8; i++){
        //add new row start
        let row_content = '<div class="row gamerow"> ';
        //add cols
        let cols = Math.round(Math.random() * 6); //max ads per row
        for(j=0; j<= cols; j++){
            //add ads
            row_content = await insertColContent(row_content);
        }
        //end row
        row_content += '</div>';
        gameboard.innerHTML += row_content;
    }
    await setDownloadButton();
   
    setTimeout(() => {}, 500);
    document.getElementById("DownloadGame").style.display="block";

}
async function pathExists(path){
    let res;
    try {
       res = await fetch("" + path,
            { method: "HEAD" }
        );
    } catch (error) {
        // console.log(error);
        return false;
    }   
    //console.log(res);
    if (res.ok){
        // console.log(path + "true");
        return true;
    }
    else{
        // console.log(path + "false");
        return false;
    }
}

async function insertColContent(row_content){
    let ad = Math.round(Math.random() * 18); //maximum ads
     //check if path exists and in case get color instead
    
    //  pathExists('images/ads/ad' + ad + '.jpg').then((exists) => {
    let result = await pathExists('images/ads/ad' + ad + '.jpg');
    // console.log("res: "+ result);
    if (result){
        // console.log("exists");
        //add image
      
        row_content += '<div class="col gamefield"> <img src="images/ads/ad' + ad + '.jpg" style="max-width: 30vw;">  </div>';
        //row_content += '<div class="col gamefield" style="images/ads/ad' + ad + '.jpg">  </div>';
        //row_content += '<div class="col gamefield" style="background-image: url("images/ads/ad' + ad + '.jpg"); background-size: cover; height: 400px; width: 100%;"></div>'
    }
    else{
        // console.log("does not exist");
        let color = "#";
        for(k=0; k < 6; k++){
            let value = Math.round(Math.random() * 16);
            if(value > 9){
                //add enough so a 10 comes to 65 (asci A)
                value += 55;
                value = String.fromCharCode(value);
            }
            color += value;
        }
        row_content += '<div class="col gamefield" style="background-color:' +  color + ';">  </div>';
    }
    return row_content;
}

async function setDownloadButton(){
    let fields = document.getElementById("DownloadGame").getElementsByClassName('gamefield');
    // console.log(fields.length);
    let button_field = Math.round(Math.random() * fields.length);
    // console.log("button: " + button_field);
    fields[button_field].innerHTML += "<span> <button class='download-btn-light'> DOWNLOAD C-MAIL </button>";
    fields[button_field].innerHTML += "<button class='download-btn-dark'> DOWNLOAD C-MAIL </button> </span>";
}