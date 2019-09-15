import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private customerData = {};
  private url = 'http://localhost:52131/api/';

  constructor(private http: HttpClient) {
  }

  public add(val) {
    this.customerData = {...this.customerData, ...val};
  }

  public getCustomerData(): any {
    return this.customerData;
  }

  public getCustomerDataById(id) {

    var json = {
      PersonID: id
    };
    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    };

    return this.http.post(this.url + "CustomSelfDeclaration/GetUserInfo", json, httpOptions);
  }

  public getGuestDataById(id) {

    var json = {
      PersonID: id
    }
    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    };

    return this.http.post(this.url + "CustomSelfDeclaration/GetGuestInfo", json, httpOptions);
  }

  public registerInfo() {
    console.log(this.customerData);
    return this.http.post(this.url, this.customerData);
  }
}
