import {Injectable} from '@angular/core';
import {User} from '../_models/user';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
    constructor(private userService: UserService, private router: Router,
        private alertify: AlertifyService, private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
// import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
// import { User } from '../_models/User';
// import { Injectable } from '@angular/core';
// import { UserService } from '../../../_services/user.service';
// import { AlertifyService } from '../../../_services/alertify.service';
// import { Observable } from 'rxjs/Observable';
// import 'rxjs/add/operator/catch';

// @Injectable()
// export class MemberDetailResolver implements Resolve<User> {
//     constructor(private userService: UserService,
//         private router: Router,
//         private alertify: AlertifyService) {

//         }

//         resolve(route: ActivatedRouteSnapshot): Observable<User> {
//             return this.userService.getUser(route.params['id']).catch(error => {
//                 this.alertify.error('Problem Retrieving Data');
//                 this.router.navigate(['/members']);
//                 return Observable.of(null);
//             });
//         }
// }
