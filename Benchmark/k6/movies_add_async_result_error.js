import http from "k6/http";
import { check } from "k6";

export const options = {
    vus: 10,
    duration: '10s',
    insecureSkipTLSVerify: true
};

export default function() {
    const url = "http://localhost:5284/api/Movies/v3";
    const payload = JSON.stringify({
        "id": 1,
        "title": "",
        "director": "Director 1",
        "releaseYear": 2020
    });
    const params = {
        headers: {
            "Content-Type": "application/json"
        }
    };
    const res = http.post(url, payload, params);

    check(res, {
        "status is 400": (r) => r.status === 400,
        "response body is not empty": (r) => r.body.length > 0
    });
}
