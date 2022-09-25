export function showJson(argJson) {
    document.getElementById("json_content").innerHTML = JSON.stringify(argJson, null, 4);
}