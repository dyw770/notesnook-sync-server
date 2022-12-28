/*
This file is part of the Notesnook Sync Server project (https://notesnook.com/)

Copyright (C) 2022 Streetwriters (Private) Limited

This program is free software: you can redistribute it and/or modify
it under the terms of the Affero GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
Affero GNU General Public License for more details.

You should have received a copy of the Affero GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Threading;
using System;
using System.Threading.Tasks;

namespace MongoDB.Driver
{
    public static class TransactionHelper
    {
        public static async Task StartTransaction(this IMongoClient client, Action<CancellationToken> operate, CancellationToken ct)
        {
            using (var session = await client.StartSessionAsync())
            {
                var transactionOptions = new TransactionOptions(readPreference: ReadPreference.Nearest, readConcern: ReadConcern.Local, writeConcern: WriteConcern.WMajority);
                await session.WithTransactionAsync((handle, token) =>
                {
                    return Task.Run(() =>
                    {
                        operate(token);
                        return true;
                    });
                }, transactionOptions, ct);
            }
        }
    }
}