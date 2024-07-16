export class RegisterWithExternals {
    firstName: string
    lastName: string
    provider: string
    accessToken: string
    userId: string

    constructor(firstName : string , lastName : string , provider: string , accessToken : string , userId :string){
        this.firstName = firstName
        this.lastName = lastName
        this.provider = provider
        this.accessToken = accessToken
        this.userId = userId

    }

}