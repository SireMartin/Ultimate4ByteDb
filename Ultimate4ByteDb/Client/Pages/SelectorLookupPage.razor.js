export function showJson(argJson) {
    document.getElementById("json_content").innerHTML = JSON.stringify(argJson, null, 4);
}
export function clearJson() {
    document.getElementById("json_content").innerHTML = "";
}