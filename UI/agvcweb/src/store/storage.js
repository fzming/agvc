export function localStorage(key, value, type) {
  switch (type) {
    case "set":
      {
        console.log("set");
        window.localStorage.setItem(key, JSON.stringify(value));
      }
      break;
    case "get": {
      return JSON.parse(window.localStorage.getItem(key));
    }
    case "remove":
      {
        window.localStorage.removeItem(key);
      }
      break;
  }
}
